using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions; // Required for 'ContinueWithOnMainThread'

public class LeaderboardManager : MonoBehaviour
{
    private DatabaseReference dbReference;

    void Start()
    {
        // 1. Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Firebase is ready, get the reference to the database
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase Connected!");
                
                // EXAMPLE USAGE:
                // WriteScore("PlayerOne", 500);
                // GetTopScores(); 
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }

    // 2. Write Data: Save Name and Score
    public void WriteScore(string userName, int score)
    {
        // We use the Device ID as the key so a user updates their own score 
        // instead of creating a new entry every time.
        string userId = SystemInfo.deviceUniqueIdentifier;
        
        // Create a user object (Class defined below)
        UserScore user = new UserScore(userName, score);
        string json = JsonUtility.ToJson(user);

        // Save to the "scores" folder in the database
        dbReference.Child("scores").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task => 
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Score saved successfully!");
                }
            });
    }

    // 3. Read Data: Get Top 10 High Scores
    public void GetTopScores()
    {
        // Query the "scores" folder, order by "score", and take the last 10 
        // (Because Firebase sorts Ascending, the "Last" ones are the highest)
        dbReference.Child("scores").OrderByChild("score").LimitToLast(10)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error loading scores");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    // The data comes back in Ascending order (Low -> High)
                    // We need to reverse it to show High -> Low
                    List<UserScore> leaderboardList = new List<UserScore>();

                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        string json = child.GetRawJsonValue();
                        UserScore userScore = JsonUtility.FromJson<UserScore>(json);
                        leaderboardList.Add(userScore);
                    }

                    // Reverse the list to get Descending order
                    leaderboardList.Reverse();

                    // Print the results
                    Debug.Log("--- LEADERBOARD ---");
                    foreach (var entry in leaderboardList)
                    {
                        Debug.Log($"{entry.name}: {entry.score}");
                    }
                }
            });
    }
}

// 4. Data Class: Simple structure to hold our data
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