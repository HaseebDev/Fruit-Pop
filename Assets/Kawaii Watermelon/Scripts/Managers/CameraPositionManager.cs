using UnityEngine;
using System.IO;

public class CameraPositionManager : MonoBehaviour
{
    public static CameraPositionManager Instance;
    private Camera mainCamera;
    private string savePath;
    private float saveInterval = 10f;
    private float elapsedTime = 0f;

    void Start()
    {
        Instance = this;
        mainCamera = Camera.main;
        savePath = Application.persistentDataPath + "/camera_position.json";

        // Load the camera position when the scene starts
        LoadCameraPosition();

        Invoke(nameof(PreiodicSave), saveInterval);
    }

    void PreiodicSave()
    {

        InvokeRepeating(nameof(SaveCameraPosition), 0f, saveInterval);

    }

    //void Update()
    //{
    //    elapsedTime += Time.deltaTime;

    //    // Check if the elapsed time has reached the save interval
    //    if (elapsedTime >= saveInterval)
    //    {
    //        SaveCameraPosition();
    //        elapsedTime = 0f; // Reset the timer
    //    }
    //}

    public void SaveCameraPosition()
    {
        TargetFruitAnimation targetAnimations = FindObjectOfType<TargetFruitAnimation>();

        if (targetAnimations != null && targetAnimations.isAnimating)
            return;

        CameraPositionData data = new CameraPositionData(mainCamera.transform.position);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);

        // Debug log for the JSON data being saved
        Debug.Log("Camera position saved: " + json);
    }

    void LoadCameraPosition()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CameraPositionData data = JsonUtility.FromJson<CameraPositionData>(json);
            mainCamera.transform.position = data.position;

            // Debug log for the JSON data being loaded
            Debug.Log("Camera position loaded: " + json);
        }
        else
        {
            Debug.Log("No saved camera position found.");
        }
    }

    public void ClearGameData()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Saved camera position data cleared.");
        }
        else
        {
            Debug.Log("No saved camera position data to clear.");
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
