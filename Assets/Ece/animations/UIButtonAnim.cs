using UnityEngine;

public class UIButtonAnim : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Bunu Button OnClick'ten çağıracağız
    public void PlayClick()
    {
        if (anim != null)
        {
            anim.SetTrigger("Click");
        }
    }
}
