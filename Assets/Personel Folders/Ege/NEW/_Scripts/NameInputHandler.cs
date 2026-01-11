using UnityEngine;
using TMPro;

public class NameInputHandler : MonoBehaviour
{
    private TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        
        // 1. Load the last used name so they don't have to type it every time
        string savedName = PlayerPrefs.GetString("PlayerName", "Driver");
        inputField.text = savedName;
        
        // 2. Sync it with GameManager immediately
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerName = savedName;
        }

        // 3. Add listener to update whenever user types
        inputField.onValueChanged.AddListener(UpdateName);
    }

public void UpdateName(string newName)
{
    // This saves the name to the computer's hard drive instantly
    PlayerPrefs.SetString("PlayerName", newName);
    PlayerPrefs.Save(); 
}
}