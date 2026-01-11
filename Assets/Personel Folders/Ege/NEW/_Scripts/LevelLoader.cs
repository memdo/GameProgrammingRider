using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

   public static LevelLoader instance;

   void Awake()
   {
      instance = this;
   }

   public Animator transition;
    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadNextLevel()
    {
      StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadMainMenu()
    {
      StartCoroutine(LoadLevel(0));
    }

   IEnumerator LoadLevel(int levelIndex)
   {
      transition.SetTrigger("Start");
      yield return new WaitForSeconds(1);
      SceneManager.LoadScene(levelIndex);
   }
}
