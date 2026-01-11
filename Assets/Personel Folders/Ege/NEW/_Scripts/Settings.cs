using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider master;
    public Slider music;
    public Slider sfx;

    void Start()
    {
        master.value = PlayerPrefs.GetFloat("Master", 1f);
        music.value  = PlayerPrefs.GetFloat("Music",  1f);
        sfx.value    = PlayerPrefs.GetFloat("SoundFX",    1f);

        master.onValueChanged.AddListener(AudioManager.Instance.SetMaster);
        music.onValueChanged.AddListener(AudioManager.Instance.SetMusic);
        sfx.onValueChanged.AddListener(AudioManager.Instance.SetSFX);
    }
}
