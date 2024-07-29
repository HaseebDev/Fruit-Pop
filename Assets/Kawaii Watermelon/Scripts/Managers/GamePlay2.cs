using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlay2 : MonoBehaviour
{
    private float totalTimeElapsed = 0f;
    public bool TimerFreezer;
    public TextMeshProUGUI TimerText;
    [HideInInspector]
    public float targetTime;
    public float initialTargetTime;
    private bool isGameFinished;
    public Levels[] levels;
    public GameObject item1;
    public GameObject item2;
    int task1;
    int task2;
    public GameObject LevelUp;
    public GameObject gameOverPanel;
    bool istask1Complete;
    bool istask2Complete;
    public Button btn;
    [HideInInspector] public bool TimerStop;
    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex != 5)
        {
            this.gameObject.SetActive(false);
           
        }
        if (Time.timeScale == 0) Time.timeScale = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        CalculateInitialTargetTime();
        targetTime = initialTargetTime;
        GenerateLevels();
        LevelUp.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        LevelUp.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => LevelIncrease());
        btn.onClick.AddListener(() => GameOver());
    }
    private void GameOver()
    {
        SceneManager.LoadScene(5);
    }
    private void LevelIncrease()
    {
      
        FindObjectOfType<LevelManager>().CloseLevelUpPanel();  
    }
    // Update is called once per frame
    void Update()
    {
        if (!isGameFinished && !TimerStop)
        {
            Timer();
        }
    }
    private void CalculateInitialTargetTime()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentActiveLevel", 1);
        initialTargetTime = (currentLevel * 10f) + 60f;
    }
    public void Timer()
    {
        if (!TimerFreezer)
        {
            totalTimeElapsed += Time.deltaTime;
            targetTime -= Time.deltaTime;
        /*    if (targetTime < 10f && RunClockSound)
            {
                PlayClip(ClockTickingSoundEffect);
                RunClockSound = true;
            }*/
            if (targetTime <= 0f)
            {
                SetLevel();
                    gameOverPanel.SetActive(true);
                FindObjectOfType<GameoverManager>().GameOverSoundPlay();
                isGameFinished = true;
                return;
            }
          //  timeFillImage.fillAmount = targetTime / initialTargetTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(targetTime);
            TimerText.text = timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        }

    }
    private void SetLevel()
    {
        if (istask1Complete && istask2Complete)
        {
            PlayerPrefs.SetInt("Level" + PlayerPrefs.GetInt("CurrentActiveLevel", 1), 3);
        }
        else if (istask1Complete)
        {
            PlayerPrefs.SetInt("Level" + PlayerPrefs.GetInt("CurrentActiveLevel", 1), 1);
        }
        else if (istask2Complete)
        {
            PlayerPrefs.SetInt("Level" + PlayerPrefs.GetInt("CurrentActiveLevel", 1), 1);
        }
        else
        {
            PlayerPrefs.SetInt("Level" + PlayerPrefs.GetInt("CurrentActiveLevel", 1), 0);
        }
    }
    public void CheckCompleteTask(string name)
    {
        if (levels[PlayerPrefs.GetInt("CurrentActiveLevel", 1) - 1].fruit[0].fruitType.ToString() == name)
        {
            if (!istask1Complete)
            {
                item1.transform.GetChild(task1).GetChild(0).gameObject.SetActive(true);
                task1++;
            }
            if (levels[PlayerPrefs.GetInt("CurrentActiveLevel", 1) - 1].fruit[0].collect == task1)
            {
                Debug.Log("task 1 is Complete");
                istask1Complete = true;
                SetLevel();
                GameFinished();
            }
            
        }
        if (levels[PlayerPrefs.GetInt("CurrentActiveLevel", 1) - 1].fruit[1].fruitType.ToString() == name)
        {
            if (!istask2Complete)
            {
                item2.transform.GetChild(task2).GetChild(0).gameObject.SetActive(true);
                task2++;
            }
            if (levels[PlayerPrefs.GetInt("CurrentActiveLevel", 1) - 1].fruit[1].collect == task2)
            {
                Debug.Log("task  is Complete");
                istask2Complete = true;
                SetLevel();
                GameFinished();
            }

            
        }
        Debug.Log(levels[PlayerPrefs.GetInt("CurrentActiveLevel", 1) - 1].fruit[0].collect + " ========= " + levels[PlayerPrefs.GetInt("CurrentActiveLevel", 1) - 1].fruit[1].collect + "  " + task1 + " " + task2);
        GameFinished();
    }
    private void GameFinished()
    {
        if (istask1Complete && istask2Complete && !isGameFinished)
        {
            SetLevel();
            isGameFinished = true;
            FindObjectOfType<LevelManager>().LevelUpGamePlay2();
            if (PlayerPrefs.GetInt("TotalLevel", 1) > PlayerPrefs.GetInt("CurrentActiveLevel", 1))
            {
                if(PlayerPrefs.GetInt("CurrentActiveLevel", 1) == PlayerPrefs.GetInt("CurrentLevel", 1))
                {
                    PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel", 1) + 1);
                    PlayerPrefs.SetInt("CurrentActiveLevel", PlayerPrefs.GetInt("CurrentActiveLevel", 1) + 1);
                }
                else
                {
                    PlayerPrefs.SetInt("CurrentActiveLevel", PlayerPrefs.GetInt("CurrentActiveLevel", 1) + 1);
                }
                PlayerPrefs.Save();
               
            }

        }

    }
    private void GenerateLevels()
    {
        for (int j = 0; j < 2; j++)
        {
            int count = levels[PlayerPrefs.GetInt("CurrentActiveLevel", 1) - 1].fruit[j].collect;
            Sprite sprite = levels[PlayerPrefs.GetInt("CurrentActiveLevel", 1) - 1].fruit[j].value;
            for (int i = 0; i < count; i++)
            {
                if(j == 0)
                {
                    item1.transform.GetChild(i).GetComponent<Image>().sprite = sprite;
                    item1.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    item2.transform.GetChild(i).GetComponent<Image>().sprite = sprite;
                    item2.transform.GetChild(i).gameObject.SetActive(true);
                }
                
            }
        }
       
       

    }
}

