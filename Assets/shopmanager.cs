using UnityEngine;
using UnityEngine.SceneManagement;

public class shopmanager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject shopUI;

    private void Start()
    {
        shopUI.SetActive(false);
    }

    // Called when pause button is pressed
    public void OpenShopMenu()
    {
        shopUI.SetActive(true);
    }

    // Continue Button
    public void Continue()
    {
        shopUI.SetActive(false);
    }


}
