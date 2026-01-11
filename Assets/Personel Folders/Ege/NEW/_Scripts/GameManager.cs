using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool isGameEnded = false;

    // --- CHANGED: SPAWN SETTINGS ---
    [Header("Vehicle Spawning Settings")]
    public GameObject[] vehiclePrefabs; 
    public Vector3 spawnCoordinates = new Vector3(0, 0, 0); // Type your X, Y, Z here in Inspector

    // --- Coin Variables ---
    private int currentRunCoins = 0;
    private int totalCoins = 0;
    public int lastRunCoins = 0;
    public TextMeshProUGUI coinText;
    private const string TOTAL_COINS_KEY = "TotalCoins";
    
    public int CurrentRunCoins => currentRunCoins;

    // --- Score Variables ---
    private int currentRunScore = 0;
    public int lastRunScore = 0; 
    private float maxDistanceTraveled = 0f;
    public TextMeshProUGUI scoreText;
    public Transform playerVehicleTransform;
    
    // Public property for distance tracking (used by ChunkManager for progressive difficulty)
    public float MaxDistanceTraveled => maxDistanceTraveled;

    // --- FIREBASE LEADERBOARD SETUP ---
    [Header("Firebase Settings")]
    public string playerName = "Anonymous"; 
    private DatabaseReference dbReference;
    private bool isFirebaseReady = false;

    public List<UserScore> globalLeaderboard = new List<UserScore>();

    public AudioClip coinSoundClip;
    private AudioSource audioSource;

    [System.Serializable]
    public class UserScore
    {
        public string name;
        public int score;

        public UserScore(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();

            LoadCoins();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                isFirebaseReady = true;
                Debug.Log("Firebase Connected Successfully.");
                FetchGlobalLeaderboard();
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
            }
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnPlayerVehicle();
        AssignUIElements();
        StartNewRun();
    }

    // --- UPDATED: SPAWN LOGIC USING COORDINATES ---
    private void SpawnPlayerVehicle()
    {
        int selectedIndex = PlayerPrefs.GetInt("selectedBike", 0);

        if (vehiclePrefabs != null && vehiclePrefabs.Length > 0)
        {
            if (selectedIndex >= vehiclePrefabs.Length) selectedIndex = 0;

            // Uses the coordinates you typed in the Inspector
            Instantiate(vehiclePrefabs[selectedIndex], spawnCoordinates, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No Vehicle Prefabs assigned in GameManager!");
        }
    }

private void AssignUIElements()
    {
        // 1. Find UI Elements (Existing code)
        TextMeshProUGUI[] textComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (TextMeshProUGUI textComp in textComponents)
        {
            if (textComp.gameObject.name == "CoinText" && textComp.transform.parent != null)
            {
                coinText = textComp;
            }
            else if (textComp.gameObject.name == "Score")
            {
                scoreText = textComp;
            }
        }

        // 2. Find the Player we just spawned
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerVehicleTransform = playerObj.transform;

            // --- NEW CODE: CONNECT CAMERA TO CAR ---
            // Find the camera script in the scene
            CameraFollow camScript = FindFirstObjectByType<CameraFollow>();
            if (camScript != null)
            {
                // Tell the camera: "This is the car you need to follow!"
                camScript.target = playerVehicleTransform;
            }
            else
            {
                Debug.LogWarning("No 'CameraFollow' script found on Main Camera!");
            }
            // ---------------------------------------
        }
        else 
        {
            Debug.LogWarning("Player tag not found! Make sure your Vehicle Prefabs are tagged 'Player'.");
        }
    }

    void Update()
    {
        if (playerVehicleTransform != null)
        {
            float currentX = Mathf.Max(0, playerVehicleTransform.position.x);

            if (currentX > maxDistanceTraveled)
            {
                maxDistanceTraveled = currentX;
            }

            int distanceScore = Mathf.FloorToInt(maxDistanceTraveled * 10f);
            currentRunScore = distanceScore + currentRunCoins;

            UpdateScoreUI();
        }
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        
        currentRunCoins += amount;
        UpdateCoinUI();
    }

    public void EndRun()
    {
        if (isGameEnded) return;

        isGameEnded = true;

        lastRunScore = currentRunScore;
        lastRunCoins = currentRunCoins;

        totalCoins += currentRunCoins;
        SaveCoins();

        if (isFirebaseReady)
        {
            CheckAndUploadHighScore(lastRunScore);
        }

        Invoke("ResetRun", 2f);
    }

    public void StartNewRun()
    {
        isGameEnded = false;
        ResetRun();
        UpdateCoinUI();
        UpdateScoreUI();
    }

    private void ResetRun()
    {
        currentRunCoins = 0;
        currentRunScore = 0;
        maxDistanceTraveled = 0f;
    }

    private void CheckAndUploadHighScore(int scoreToUpload)
    {
        if (scoreToUpload <= 0) return;

        string userId = SystemInfo.deviceUniqueIdentifier;
        string finalName = PlayerPrefs.GetString("PlayerName", "Unknown Driver");
        playerName = finalName;

        dbReference.Child("scores").Child(userId).GetValueAsync().ContinueWithOnMainThread(task => 
        {
            if (task.IsFaulted) return;

            DataSnapshot snapshot = task.Result;

            if (snapshot.Exists)
            {
                string json = snapshot.GetRawJsonValue();
                UserScore oldData = JsonUtility.FromJson<UserScore>(json);

                if (scoreToUpload > oldData.score)
                {
                    WriteScoreToDB(userId, finalName, scoreToUpload);
                }
            }
            else
            {
                WriteScoreToDB(userId, finalName, scoreToUpload);
            }
        });
    }

    private void WriteScoreToDB(string userId, string name, int score)
    {
        UserScore user = new UserScore(name, score);
        string json = JsonUtility.ToJson(user);

        dbReference.Child("scores").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(writeTask => 
            {
                if (writeTask.IsCompleted)
                {
                    FetchGlobalLeaderboard();
                }
            });
    }

    public void FetchGlobalLeaderboard()
    {
        if (!isFirebaseReady) return;

        dbReference.Child("scores").OrderByChild("score").LimitToLast(10)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    globalLeaderboard.Clear();

                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        string json = child.GetRawJsonValue();
                        UserScore userScore = JsonUtility.FromJson<UserScore>(json);
                        globalLeaderboard.Add(userScore);
                    }
                    globalLeaderboard.Reverse();
                }
            });
    }

    public void UpdateCoinUI()
    {
        if (coinText != null) coinText.text = currentRunCoins.ToString();
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "" + currentRunScore.ToString();
    }

    public void SaveCoins()
    {
        PlayerPrefs.SetInt(TOTAL_COINS_KEY, totalCoins);
        PlayerPrefs.Save();
    }

    public void LoadCoins()
    {
        totalCoins = PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
    }

    public void PlayCoinSound()
    {
        if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(coinSoundClip);
    }

    // --- NEW: Helper to see the spawn point in the Editor Scene View ---
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnCoordinates, 0.5f);
    }
}