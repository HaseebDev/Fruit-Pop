using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;
    public GameObject[] backgrounds;  // Array to hold your backgrounds
    public Sprite[] backgroundSprites;  // Array to hold your backgrounds
    private float backgroundHeight = 10.2f;  // Height difference between backgrounds
    int BackGroundNumber = 0;
    int check = 0; // This variable is used within your logic
    int moveCount = 0; // New counter to keep track of the number of times backgrounds are moved

    private string saveFileName = "background_data.json";
    private float saveInterval = 10f; // Time interval for periodic saves
    private float saveTimer = 0f;
    private int currentActiveBackground;
    void Start()
    {
        Instance = this;
        if (!PlayerPrefs.HasKey("CheckKey"))
        {
            check = 0;
            PlayerPrefs.SetInt("CheckKey", check);
        }
        else
        {
            check = PlayerPrefs.GetInt("CheckKey");
        }
        // PlayerPrefs.SetInt("SelectedBg", 1);
        currentActiveBackground = PlayerPrefs.GetInt("SelectedBg");
        // Initial placement of backgrounds
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].transform.position = new Vector2(0, -i * backgroundHeight);
            backgrounds[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = backgroundSprites[currentActiveBackground];
        }
        LoadBackgroundData();
        Invoke(nameof(PreiodicSave), saveInterval);

    }

    public void MoveToNextLevel()
    {
        MoveBackgroundToEnd(BackGroundNumber);
    }

    private void MoveBackgroundToEnd(int completedIndex)
    {
        if (completedIndex < 0 || completedIndex >= backgrounds.Length) return;

        GameObject completedBackground = backgrounds[completedIndex];

        // Move the completed background to the end
        for (int i = completedIndex; i < backgrounds.Length - 1; i++)
        {
            backgrounds[i] = backgrounds[i + 1];
        }
        backgrounds[backgrounds.Length - 1] = completedBackground;

        Vector2 newPosition = new Vector2(0, -(backgrounds.Length + check) * backgroundHeight);
        completedBackground.transform.position = newPosition;
        Debug.Log("BackgroundMoved");
        check++;
        PlayerPrefs.SetInt("CheckKey", check);
    }
    void PreiodicSave()
    {


        InvokeRepeating(nameof(SaveBackgroundData), 0f, saveInterval);

    }
    //private void Update()
    //{
    //    // Update the save timer
    //    saveTimer += Time.deltaTime;

    //    // Check if it's time to save
    //    if (saveTimer >= saveInterval)
    //    {
    //        SaveBackgroundData();
    //        saveTimer = 0f; // Reset the timer
    //    }
    //}

    public void SaveBackgroundData()
    {

        TargetFruitAnimation targetAnimations = FindObjectOfType<TargetFruitAnimation>();

        if (targetAnimations != null && targetAnimations.isAnimating)
            return;

        List<SerializableBackgroundData> backgroundsData = new List<SerializableBackgroundData>();

        for (int i = 0; i < backgrounds.Length; i++)
        {
            SerializableBackgroundData backgroundData = new SerializableBackgroundData();
            backgroundData.name = backgrounds[i].name;
            backgroundData.position = new SerializableVector3(backgrounds[i].transform.position);
            backgroundData.index = i;
            backgroundsData.Add(backgroundData);
        }

        string jsonData = JsonConvert.SerializeObject(backgroundsData);
        Debug.Log("Background data being saved: " + jsonData); // Log the JSON data
        File.WriteAllText(GetFilePath(), jsonData);

        Debug.Log("Background data saved.");
    }


    private void LoadBackgroundData()
    {
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            List<SerializableBackgroundData> backgroundsData = JsonConvert.DeserializeObject<List<SerializableBackgroundData>>(jsonData);

            foreach (var backgroundData in backgroundsData)
            {
                // Find the background GameObject by name
                GameObject background = GameObject.Find(backgroundData.name);
                if (background != null)
                {
                    // Update the position of the background GameObject
                    background.transform.position = backgroundData.position.ToVector3();
                    // Update the index of the background GameObject
                    backgrounds[backgroundData.index] = background;
                }
                else
                {
                    Debug.LogWarning("Background with name " + backgroundData.name + " not found.");
                }
            }
        }
    }





    public void ClearBackgroundData()
    {
        File.Delete(GetFilePath());
        PlayerPrefs.DeleteKey("CheckKey");
    }

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, saveFileName);
    }

    [System.Serializable]
    public class SerializableBackgroundData
    {
        public string name;
        public SerializableVector3 position;
        public int index;
    }



    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

}
