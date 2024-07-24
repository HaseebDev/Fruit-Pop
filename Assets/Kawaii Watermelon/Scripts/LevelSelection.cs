using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] TMP_Text commonCurrencyText;
    [SerializeField] TMP_Text rareCurrencyText;
    [SerializeField] AudioSource musicSource;
    public GameObject LevelPrefab;
    public int LevelCount;
    public Transform Content;
    Vector3 currentPosition;
    public List<int> levelEdge = new List<int> { 3, 6, 10, 15, 19, 25 };
    string direaction = "left";
    int positionCounter;
    // Start is called before the first frame update
    void Start()
    {
         UpdateCurrencyUi();
         InitialSetupMusicAudio();
        PlayerPrefs.SetInt("CurrentLevel", 14);
        for(int i = 0; i < LevelCount; i++)
        {
           
            if (positionCounter == 0)
            {
                currentPosition = new Vector3(0, -(positionCounter + 1) * 100, 0);
            }
            else
            {
                if(positionCounter == 1)
                currentPosition = new Vector3(0, -(positionCounter + 1 - 1) * 400, 0);
                else
                currentPosition = new Vector3(0, -((positionCounter + 1 - 1) * 300) - 100, 0);
            }
            positionCounter++;
            if (levelEdge.Contains(i))
            {
                GameObject item1 = Instantiate(LevelPrefab, Content);
                item1.GetComponent<Transform>().localPosition = currentPosition;       
                SetLevel(item1,i, "middle");
                i++;
                GameObject item2 = Instantiate(LevelPrefab, Content);
                if (direaction.Equals("left"))
                {
                    item2.GetComponent<Transform>().localPosition = currentPosition + new Vector3(-265, 0, 0);
                    direaction = "right";
                    item2.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                    item2.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    item1.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    item2.GetComponent<Transform>().localPosition = currentPosition + new Vector3(265, 0, 0);
                    direaction = "left";
                    item1.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    item2.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                    item2.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                }        
                SetLevel(item2,i, "edge");
               // currentPosition = new Vector3(0, - (i - 1) * 350, 0);

            }
            else
            {
                GameObject item =  Instantiate(LevelPrefab, Content);
                item.GetComponent<Transform>().localPosition = currentPosition;
                if (i == 0)
                {
                    item.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                }
                item.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                item.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                SetLevel(item,i, "top");
            }
            
        }
        setContentHeight(Content.GetChild(Content.childCount - 1).transform.localPosition.y - 200);
    }
    private void setContentHeight(float height)
    {
        RectTransform contentRectTransform = Content.GetComponent<RectTransform>();
        if (contentRectTransform != null)
        {
            Vector2 sizeDelta = contentRectTransform.sizeDelta;
            sizeDelta.y = -1 * height;
            contentRectTransform.sizeDelta = sizeDelta;
            contentRectTransform.DOMoveY(-1 * height, 1);
        }
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
        if(AdsCurrencyManager.instance != null)
        {
            AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);
            AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
        }
    }
    private void SetLevel(GameObject item,int i,string dir)
    {
        if (LevelCount - i <= PlayerPrefs.GetInt("CurrentLevel", 1))
        {
            if (LevelCount - i < PlayerPrefs.GetInt("CurrentLevel", 1))
            {
                item.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                item.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                switch (dir)
                {
                    case "edge":
                        item.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = ((LevelCount - i) + 1).ToString();
                        break;
                    case "middle":
                        item.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = ((LevelCount - i) - 1).ToString();
                        break;
                    case "top":
                        item.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = (LevelCount - i).ToString();
                        break;
                }
                
            
            }
            
            if (LevelCount - i == PlayerPrefs.GetInt("CurrentLevel", 1))
            {
                Debug.Log("LevelCount - i  " + (LevelCount - i) + " , " + PlayerPrefs.GetInt("CurrentLevel", 1));
                item.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                item.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>().text = (LevelCount - i).ToString();
                item.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            }
        }
        else
         {
                item.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                item.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                item.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
         }
       

    }
    public void BackBtn()
    {
        SceneManager.LoadScene(2);
    }


}
