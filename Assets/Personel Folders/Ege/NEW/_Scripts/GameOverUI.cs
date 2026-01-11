using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // TextMeshPro yerine bunu kullanýyoruz

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    public Text coinText;  // TextMeshProUGUI yerine sadece Text
    public Text scoreText; // TextMeshProUGUI yerine sadece Text

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            if (coinText != null)
            {
                coinText.text = GameManager.Instance.lastRunCoins.ToString();
            }

            if (scoreText != null)
            {
                scoreText.text = GameManager.Instance.lastRunScore.ToString();
            }
        }
    }
}