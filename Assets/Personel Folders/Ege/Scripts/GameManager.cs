using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; // Required for List<T>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // The Singleton instance

    // --- Coin Variables ---
    private int currentRunCoins = 0;
    private int totalCoins = 0; 
    public TextMeshProUGUI coinText;
    private const string TOTAL_COINS_KEY = "TotalCoins";

    // --- Score Variables ---
    private int currentRunScore = 0;
    public TextMeshProUGUI scoreText;
    public Transform playerVehicleTransform; 
    
    // --- NEW LEADERBOARD SETUP ---
    private const string LEADERBOARD_KEY = "LeaderboardScores";
    private const int MAX_LEADERBOARD_ENTRIES = 5;

    // Inside GameManager.cs, add these members:
    public AudioClip coinSoundClip; // Drag your coin sound file here in the Inspector
    private AudioSource audioSource;

    // Helper class for JSON serialization (must be [System.Serializable])
    [System.Serializable]
    public class LeaderboardData
    {
        // Stores all the scores in the leaderboard
        public List<int> scores = new List<int>();
    }
    
    private LeaderboardData leaderboardData;
    
    // --- END NEW LEADERBOARD SETUP ---

    void Awake()
    {

    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // Ensure GameManager persists

        // Add and get the AudioSource component for SFX
        audioSource = gameObject.AddComponent<AudioSource>();

        LoadCoins();
        LoadLeaderboard(); // Assuming you have LoadLeaderboard() implemented

        // Listen for scene changes to reset run data
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-assign UI elements for the new scene
        AssignUIElements();

        // Reset run data when loading the game scene
        if (scene.name == "HCR Bike")
        {
            StartNewRun();
        }
    }

    // Find and assign UI elements when scene loads
    private void AssignUIElements()
    {
        // Find all TextMeshProUGUI components and assign the correct ones
        TextMeshProUGUI[] textComponents = FindObjectsOfType<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI textComp in textComponents)
        {
            // Check if this is the coin text (look for objects named CoinText that are not Canvas)
            if (textComp.gameObject.name == "CoinText" && textComp.transform.parent != null)
            {
                coinText = textComp;
                Debug.Log("Found CoinText UI element");
            }
            // Check if this is the score text
            else if (textComp.gameObject.name == "Score")
            {
                scoreText = textComp;
                Debug.Log("Found Score UI element");
            }
        }

        // Find the player vehicle for score calculation
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerVehicleTransform = playerObj.transform;
            Debug.Log("Found Player object");
        }
    }

    // --- New: Live Score Calculation ---
    void Update()
    {
        // Calculate the score based on distance traveled (X-axis)
        if (playerVehicleTransform != null)
        {
            // Only count positive distance (progress forward)
            float distance = Mathf.Max(0, playerVehicleTransform.position.x);
            
            // Score Calculation: Distance (e.g., * 10) + Coins
            // We floor it to an integer for display
            int distanceScore = Mathf.FloorToInt(distance * 10f);

            // The live score is based on distance traveled PLUS coins collected so far
            currentRunScore = distanceScore + currentRunCoins;
            
            // Update the UI
            UpdateScoreUI();
        }
    }

    public void AddCoins(int amount)
    {
        currentRunCoins += amount;
        UpdateCoinUI();
    }

    // Called when the player finishes a race/run (e.g., game over)
    public void EndRun()
    {
        // 1. Save Coins
        totalCoins += currentRunCoins;
        SaveCoins();

        // 2. Update and Save Leaderboard (NEW LOGIC)
        UpdateLeaderboard(currentRunScore);

        // 3. Reset for the next run
        ResetRun();
    }

    // Called when starting a new run/game
    public void StartNewRun()
    {
        ResetRun();
        UpdateCoinUI();
        UpdateScoreUI();
    }

    private void ResetRun()
    {
        currentRunCoins = 0;
        currentRunScore = 0;
    }

    // --- NEW LEADERBOARD METHODS ---

    private void UpdateLeaderboard(int newScore)
    {
        if (newScore > 0)
        {
            // Add the new score
            leaderboardData.scores.Add(newScore);
            
            // Sort in descending order (highest score first)
            leaderboardData.scores.Sort((a, b) => b.CompareTo(a));

            // Keep only the top 5 entries
            if (leaderboardData.scores.Count > MAX_LEADERBOARD_ENTRIES)
            {
                leaderboardData.scores.RemoveRange(MAX_LEADERBOARD_ENTRIES, leaderboardData.scores.Count - MAX_LEADERBOARD_ENTRIES);
            }

            SaveLeaderboard();
        }
    }

    public void LoadLeaderboard()
    {
        // Check if a leaderboard already exists in PlayerPrefs
        if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
        {
            string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
        }
        else
        {
            // Initialize an empty leaderboard if none is saved
            leaderboardData = new LeaderboardData();
        }
    }
    
    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("Leaderboard Saved.");
    }
    
    // Public accessor to allow the MainMenu to get the scores
    public List<int> GetLeaderboardScores()
    {
        return leaderboardData.scores;
    }
    
    // --- END NEW LEADERBOARD METHODS ---

    // --- UI Update Functions (unchanged) ---
    
    public void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = currentRunCoins.ToString();
        }
        else
        {
            Debug.LogWarning("CoinText UI element not found!");
        }
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentRunScore.ToString();
        }
        else
        {
            Debug.LogWarning("ScoreText UI element not found!");
        }
    }
    
    // --- Persistence Functions (updated) ---
    
    public void SaveCoins()
    {
        PlayerPrefs.SetInt(TOTAL_COINS_KEY, totalCoins);
        PlayerPrefs.Save();
        Debug.Log("Coins Saved: " + totalCoins);
    }
    
    public void LoadCoins()
    {
        totalCoins = PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
        Debug.Log("Coins Loaded: " + totalCoins);
    }

    // Add this new public method to play the coin sound:
    public void PlayCoinSound()
    {
        if (coinSoundClip != null && audioSource != null)
        {
            // PlayOneShot ensures the audio clip plays immediately, even if another clip is playing.
            audioSource.PlayOneShot(coinSoundClip);
        }
    }
}