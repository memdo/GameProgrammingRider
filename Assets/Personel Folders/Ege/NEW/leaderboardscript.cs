using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class leaderboardscript : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    public TextMeshProUGUI leaderboardText2;

    void Start()
    {
        leaderboardText.text = "                       Loading ";
        leaderboardText2.text = "Scores";

        // 1. Force a refresh immediately when this screen opens
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FetchGlobalLeaderboard();
        }

        // 2. Keep checking for the data to arrive every 1 second
        InvokeRepeating(nameof(UpdateLeaderboardUI), 0.5f, 1f);
    }

    void UpdateLeaderboardUI()
    {
        if (GameManager.Instance == null) return;

        var scoresList = GameManager.Instance.globalLeaderboard;

        // If still empty, keep waiting
        if (scoresList.Count == 0)
        {
            // Optional: You can change this to "No Scores Yet" if it stays empty too long
            return;
        }

        // Build the list
        string displayString = "";
        string displayString2 = "";

        
        for (int i = 0; i < 5; i++)
        {
            if(i + 1 <= scoresList.Count) displayString += $"{i + 1}. {scoresList[i].name} : {scoresList[i].score}\n";
        }

        leaderboardText.text = displayString;

        for (int i = 5; i < 10; i++)
        {
            if(i + 1 <= scoresList.Count) displayString2 += $"{i + 1}. {scoresList[i].name} : {scoresList[i].score}\n";
        }

        leaderboardText2.text = displayString2;

    }
    
    void OnDisable()
    {
        CancelInvoke();
    }
}