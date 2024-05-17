using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] backgrounds;  // Array to hold your backgrounds
    private float backgroundHeight = 10.2f;  // Height difference between backgrounds
    int BackGroundNumber = 0;


    void Start()
    {

        // Initial placement of backgrounds if needed
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].transform.position = new Vector2(0, -i * backgroundHeight);
        }
    }

    public void MoveToNextLevel()
    {
        StartCoroutine(MoveBackgroundToEnd(BackGroundNumber));
    }

    private IEnumerator MoveBackgroundToEnd(int completedIndex)
    {
        if (completedIndex < 0 || completedIndex >= backgrounds.Length) yield break;

        // Wait for 10 seconds
        yield return new WaitForSeconds(1f);

        GameObject completedBackground = backgrounds[completedIndex];

        // Move the completed background to the end
        for (int i = completedIndex; i < backgrounds.Length - 1; i++)
        {
            backgrounds[i] = backgrounds[i + 1];
        }
        backgrounds[backgrounds.Length - 1] = completedBackground;

        //// Reposition backgrounds
        //for (int i = 0; i < backgrounds.Length; i++)
        //{
        //    backgrounds[i].transform.position = new Vector2(0, -i * backgroundHeight);
        //}

        // Calculate the new position for the completed background
        Vector2 newPosition = new Vector2(0, -(backgrounds.Length + check) * backgroundHeight);
        // Move the completed background to the new position
        completedBackground.transform.position = newPosition;
        check++;
    }

    int check;
}
