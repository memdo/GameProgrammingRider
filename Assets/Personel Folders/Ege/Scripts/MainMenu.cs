using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; // Required for List<T>

public class MainMenu : MonoBehaviour
{
    // Public variable to link your UI text component for COINS in the Inspector
    public TextMeshProUGUI totalCoinText;
    
    // NEW: Text field to display the entire leaderboard
    public TextMeshProUGUI leaderboardText;
    
    // Constant keys (must match GameManager.cs)
    private const string TOTAL_COINS_KEY = "TotalCoins";
    private const string LEADERBOARD_KEY = "LeaderboardScores";

    // NEW: Helper class MUST match the structure in GameManager.cs
    [System.Serializable]
    public class LeaderboardData
    {
        public List<int> scores = new List<int>();
    }

    void Start()
    {
        // 1. Load and display the total lifetime coins
        int savedCoins = PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
        
        if (totalCoinText != null)
        {
            totalCoinText.text = "Total Coins: " + savedCoins.ToString();
        }

        // 2. Load and display the Top 5 Leaderboard (NEW)
        LoadAndDisplayLeaderboard();
    }

    private void LoadAndDisplayLeaderboard()
    {
        if (leaderboardText == null) return;
        
        // Load the JSON string from PlayerPrefs. If none exists, provide an empty JSON object.
        string json = PlayerPrefs.GetString(LEADERBOARD_KEY, "{\"scores\":[]}");
        
        // Deserialize the JSON string back into our data structure
        LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);

        // Sort again (descending order) for display robustness
        data.scores.Sort((a, b) => b.CompareTo(a)); 

        // Format the scores for display
        string leaderboardDisplay = "--- TOP 5 SCORES ---\n";
        
        for (int i = 0; i < data.scores.Count; i++)
        {
            leaderboardDisplay += (i + 1) + ". " + data.scores[i].ToString() + "\n";
        }

        // Update the UI text component
        leaderboardText.text = leaderboardDisplay;
    }

    public void StartButton()
    {
       SceneManager.LoadScene("HCR Bike");
    }

    public void QuitButton()
    {
       Application.Quit();
    }
}