using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource sound;
    public AudioSource music;
    [HideInInspector] public string MusicVolumeKey = "MusicVolume";
    [HideInInspector] public string SoundVolumeKey = "SoundVolume";
    [HideInInspector] public string MusicMuteKey = "MusicMute";
    [HideInInspector] public string SoundMuteKey = "SoundMute";

    // Declare an event to notify subscribers when music volume is updated
    public delegate void MusicVolumeUpdated(float volume);
    public static event MusicVolumeUpdated OnMusicVolumeUpdated;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(MusicVolumeKey))
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, 1f);
        }
        if (!PlayerPrefs.HasKey(SoundVolumeKey))
        {
            PlayerPrefs.SetFloat(SoundVolumeKey, 1f);
        }
        // Load the saved volume values and mute states or use default values
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
        float savedSoundVolume = PlayerPrefs.GetFloat(SoundVolumeKey);
        // Set the initial volume values based on the loaded values
        music.volume = savedMusicVolume;
        sound.volume = savedSoundVolume;
    }

    public void PlayOneShot(AudioClip clip)
    {
        sound.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        music.clip = clip;
        music.Play();
    }

    public void UpdateMusicVolume(float volume)
    {
        if (music != null)
        {
            music.volume = volume;
            // Invoke the event when music volume is updated
            if (OnMusicVolumeUpdated != null)
            {
                OnMusicVolumeUpdated.Invoke(volume);
            }
        }
    }

    public void UpdateSoundVolume(float volume)
    {
        if (sound != null)
        {
            sound.volume = volume;
        }
    }
}
