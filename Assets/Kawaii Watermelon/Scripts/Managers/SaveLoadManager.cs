using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;
    private string saveFileName = "game_data.json";

    private void Start()
    {
        if(Instance == null)
            Instance = this;

    }


    public void SaveGameData(List<FruitData> fruitsData)
    {
        string jsonData = JsonConvert.SerializeObject(fruitsData);
        Debug.Log("Serialized JSON data: " + jsonData); // Add this line to log the serialized data
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileName), jsonData);
    }

    public List<FruitData> LoadGameData()
    {
        List<FruitData> fruitsData = new List<FruitData>();

        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            Debug.Log("Loaded JSON data: " + jsonData); // Add this line to log the loaded data
            fruitsData = JsonConvert.DeserializeObject<List<FruitData>>(jsonData);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + filePath);
        }

        return fruitsData;
    }


    public void ClearGameData()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, saveFileName));
    }
    [System.Serializable]
    public class FruitData
    {
        public SerializableVector3 position;
        public int fruitIndex; // Index of the fruit in the array
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
