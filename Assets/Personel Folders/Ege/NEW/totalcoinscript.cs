using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; // Required for List<T>

public class totalcoinscript : MonoBehaviour
{
    // Public variable to link your UI text component for COINS in the Inspector
    public TextMeshProUGUI totalCoinText;
    
    
    // Constant keys (must match GameManager.cs)
    private const string TOTAL_COINS_KEY = "TotalCoins";



    void Start()
    {
        // 1. Load and display the total lifetime coins
        int savedCoins = PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
        
        if (totalCoinText != null)
        {
            totalCoinText.text = "" + savedCoins.ToString();
        }

    }
}