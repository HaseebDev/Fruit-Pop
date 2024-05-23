using UnityEngine;
using UnityEditor;
using System.IO;

public class DeletePersistentDataJsonFiles : EditorWindow
{
    [MenuItem("Tools/Delete Persistent Data JSON Files")]
    public static void ShowWindow()
    {
        GetWindow<DeletePersistentDataJsonFiles>("Delete Persistent Data JSON Files");
    }

    private void OnGUI()
    {
        GUILayout.Label("Delete JSON Files in Persistent Data Path", EditorStyles.boldLabel);

        if (GUILayout.Button("Delete All JSON Files"))
        {
            DeleteAllJsonFiles(Application.persistentDataPath);
        }
    }

    private static void DeleteAllJsonFiles(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Debug.LogError("Directory does not exist: " + directory);
            return;
        }

        string[] jsonFiles = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);

        foreach (string jsonFile in jsonFiles)
        {
            try
            {
                File.Delete(jsonFile);
                Debug.Log("Deleted: " + jsonFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error deleting file: " + jsonFile + "\n" + ex.Message);
            }
        }

        // Refresh the editor to update the changes
        AssetDatabase.Refresh();
    }
}
