using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnLevelClick : MonoBehaviour
{
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public TMP_Text txt;
    public TMP_Text txt2;
    private void Start()
    {
        int count;
        if(int.Parse(txt.text) == 0)
        {
            count = int.Parse(txt2.text);
        }
        else
        {
            count = int.Parse(txt.text);
        }
        if (PlayerPrefs.GetInt("CurrentLevel", 1) >= count)
        {
            switch (PlayerPrefs.GetInt("Level" + count, 0)) 
            {
                case 1:
                    star2.SetActive(true);
                    star1.SetActive(false);
                    star3.SetActive(false);
                    break;
                case 0:
                    star2.SetActive(false);
                    star1.SetActive(false);
                    star3.SetActive(false);
                    break;
                case 3:
                    star2.SetActive(true);
                    star1.SetActive(true);
                    star3.SetActive(true);
                    break;
            }

        }
    }
    public void LevelClick(TMP_Text txt)
    {
        if (PlayerPrefs.GetInt("CurrentLevel", 1) >= int.Parse(txt.text))
        {
            PlayerPrefs.SetInt("CurrentActiveLevel", int.Parse(txt.text));
            SceneManager.LoadScene(5);
        }
    }
}
