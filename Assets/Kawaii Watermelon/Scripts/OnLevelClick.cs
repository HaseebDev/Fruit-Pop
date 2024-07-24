using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnLevelClick : MonoBehaviour
{
    public void LevelClick(TMP_Text txt)
    {
        if (PlayerPrefs.GetInt("CurrentLevel", 1) >= int.Parse(txt.text))
        {
            SceneManager.LoadScene(5);
        }
    }
}
