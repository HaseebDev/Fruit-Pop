using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeManager : MonoBehaviour
{
    [SerializeField] TMP_Text commonCurrencyText;
    [SerializeField] TMP_Text rareCurrencyText;
    [SerializeField] AudioSource musicSource;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCurrencyUi();
        InitialSetupMusicAudio();
    }

    private void InitialSetupMusicAudio()
    {
        float MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        Debug.Log("Music Volume is " + MusicVolume);

        float mappedMusicVolume = MusicVolume * 0.25f; // Map the value to a maximum of 0.25
        musicSource.volume = mappedMusicVolume;
        Debug.Log("Adjusted Music Volume is " + mappedMusicVolume);
    }
    public void UpdateCurrencyUi()
    {
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
    }
    public void TapToPlayButton(int mode)
    {
        if(mode == 1)
        {
            SceneManager.LoadScene(3);
        }
        else if(mode == 2)
        {
            SceneManager.LoadScene(4);
        }
        
    }

    public void BackBtn()
    {
        SceneManager.LoadScene(1);
    }
}
