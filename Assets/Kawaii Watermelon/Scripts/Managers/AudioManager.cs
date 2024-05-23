using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private AudioSource mergeSource;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource[] soundSources;
    private void Awake()
    {
        MergeManager.onMergeProcessed += MergeProcessedCallback;
        //SettingsManager.onSFXValueChanged += SFXValueChangedCallback;
    }

    private void OnDestroy()
    {
        MergeManager.onMergeProcessed -= MergeProcessedCallback;
        //SettingsManager.onSFXValueChanged -= SFXValueChangedCallback;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();
    }


    void LoadSettings()
    {
        // Adjust the music volume
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        Debug.Log("Music Volume is " + musicVolume);

        float mappedMusicVolume = musicVolume * 0.25f; // Map the value to a maximum of 0.25
        musicSource.volume = mappedMusicVolume;
        Debug.Log("Adjusted Music Volume is " + mappedMusicVolume);

        // Adjust sound volume
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        Debug.Log("Sound Volume is " + soundVolume);

        foreach (AudioSource audioSource in soundSources)
        {
            audioSource.volume = soundVolume;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void MergeProcessedCallback(FruitType fruitType, Vector2 mergePos)
    {
        PlayMergeSound();
    }

    public void PlayMergeSound()
    {
        mergeSource.pitch = Random.Range(.9f, 1.1f);
        mergeSource.Play();
    }

    private void SFXValueChangedCallback(bool sfxActive)
    {
        mergeSource.mute = !sfxActive;
        //mergeSource.volume = sfxActive ? 1 : 0;
    }
}
