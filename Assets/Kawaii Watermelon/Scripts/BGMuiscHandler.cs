using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMuiscHandler : MonoBehaviour
{
    public AudioSource bgMusic;
    public static BGMuiscHandler _Instance;
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 3)
        {
            if(!bgMusic.isPlaying)
            bgMusic.Play();
        }
        else
        {
            bgMusic.Pause();
        }
    }
}
