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
        // Load the saved volume values and mute states or use default values
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float savedSoundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, 1f);
        // Set the initial volume values based on the loaded values
        music.volume =  savedMusicVolume;
        sound.volume =  savedSoundVolume;
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