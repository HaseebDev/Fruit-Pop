using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button _ReviveButton;

    private void Awake()
    {
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    // Start is called before the first frame update
    void Start()
    {
        _ReviveButton.onClick.AddListener(ReviveButton);
        PlayButtonCallback();
        //SetMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                SetMenu();
                break;

            case GameState.Game:
                SetGame();
                break;

            case GameState.Gameover:
                SetGameover();
                break;
        }
    }

    private void SetMenu()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameoverPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    private void SetGame()
    {
        gamePanel.SetActive(true);
        menuPanel.SetActive(false);
        gameoverPanel.SetActive(false);
    }

    private void SetGameover()
    {
        gameoverPanel.SetActive(true);
        FindAnyObjectByType<LevelManager>().UpdateUI();
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    public void PlayButtonCallback()
    {
        GameManager.instance.SetGameState();
        SetGame();
    }

    public void NextButtonCallback()
    {
        SaveLoadManager.Instance.ClearGameData();
        BackgroundManager.Instance.ClearBackgroundData();
        CameraPositionManager.Instance.ClearGameData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ReviveButton()
    {
        if (PlayerPrefs.GetFloat("RareCurrency") >= 12)
        {
            // Close gameover panel
            gameoverPanel.SetActive(false);
            PlayerPrefs.SetFloat("RareCurrency", PlayerPrefs.GetFloat("RareCurrency") - 12);
            FindAnyObjectByType<LevelManager>().UpdateUI();
            // Destroy fruits above or touching the deadline
            Fruit[] fruits = FindObjectsOfType<Fruit>();
            foreach (Fruit fruit in fruits)
            {
                if (fruit.transform.position.y >= FindAnyObjectByType<GameoverManager>().deadLine.transform.position.y)
                {
                    Destroy(fruit.gameObject);
                }
            }

            // Reset game state or perform any other necessary actions
            GameManager.instance.SetGameState();
        }
        else
        {
            _ReviveButton.interactable = false;
        }

    }

    public void SettingsButtonCallback()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    private void OnApplicationQuit()
    {

        if (gameoverPanel.activeInHierarchy == true)
        {
            SaveLoadManager.Instance.ClearGameData();
            BackgroundManager.Instance.ClearBackgroundData();
            CameraPositionManager.Instance.ClearGameData();
        }
        else
        {
            FruitManager.Instance.SaveFruitPositions();
            CameraPositionManager.Instance.SaveCameraPosition();
            BackgroundManager.Instance.SaveBackgroundData();
        }
    }
}
