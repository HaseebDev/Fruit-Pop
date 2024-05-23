using UnityEngine;
using UnityEditor;
using System.IO;

public class DeleteJsonFiles : EditorWindow
{
    private string targetDirectory = "Assets"; // Default to the Assets folder. Change as needed.

    [MenuItem("Tools/Delete JSON Files")]
    public static void ShowWindow()
    {
        GetWindow<DeleteJsonFiles>("Delete JSON Files");
    }

    private void OnGUI()
    {
        GUILayout.Label("Delete JSON Files", EditorStyles.boldLabel);

        targetDirectory = EditorGUILayout.TextField("Target Directory", targetDirectory);

        if (GUILayout.Button("Delete All JSON Files"))
        {
            DeleteAllJsonFiles(targetDirectory);
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

        AssetDatabase.Refresh();
    }
}
