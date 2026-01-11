using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer mixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    const string MASTER = "Master";
    const string MUSIC  = "Music";
    const string SFX    = "SoundFX";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumes();
    }

    void LoadVolumes()
    {
        Apply(MASTER, PlayerPrefs.GetFloat(MASTER, 1f));
        Apply(MUSIC,  PlayerPrefs.GetFloat(MUSIC,  1f));
        Apply(SFX,    PlayerPrefs.GetFloat(SFX,    1f));
    }

    void Apply(string param, float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mixer.SetFloat(param, Mathf.Log10(value) * 20f);
    }

    // ===== PUBLIC API =====

    public void SetMaster(float value) => Set(MASTER, value);
    public void SetMusic(float value)  => Set(MUSIC,  value);
    public void SetSFX(float value)    => Set(SFX,    value);

    void Set(string param, float value)
    {
        Apply(param, value);
        PlayerPrefs.SetFloat(param, value);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
