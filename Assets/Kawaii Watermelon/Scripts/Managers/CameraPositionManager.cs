using UnityEngine;
using System.IO;

public class CameraPositionManager : MonoBehaviour
{
    private Camera mainCamera;
    private string savePath;
    private float saveInterval = 30f;
    private float elapsedTime = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        savePath = Application.persistentDataPath + "/camera_position.json";

        // Load the camera position when the scene starts
        LoadCameraPosition();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Check if the elapsed time has reached the save interval
        if (elapsedTime >= saveInterval)
        {
            SaveCameraPosition();
            elapsedTime = 0f; // Reset the timer
        }
    }

    void SaveCameraPosition()
    {
        CameraPositionData data = new CameraPositionData(mainCamera.transform.position);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    void LoadCameraPosition()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CameraPositionData data = JsonUtility.FromJson<CameraPositionData>(json);
            mainCamera.transform.position = data.position;
        }
    }

    [System.Serializable]
    private class CameraPositionData
    {
        public Vector3 position;

        public CameraPositionData(Vector3 position)
        {
            this.position = position;
        }
    }
}
