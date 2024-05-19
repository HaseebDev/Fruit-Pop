using System.Collections;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] backgrounds;  // Array to hold your backgrounds
    private float backgroundHeight = 10.2f;  // Height difference between backgrounds
    int BackGroundNumber = 0;
    int check = 0; // This variable is used within your logic
    int moveCount = 0; // New counter to keep track of the number of times backgrounds are moved

    void Start()
    {
        // Initial placement of backgrounds
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

        // Wait for 1 second (adjust as needed)
        yield return new WaitForSeconds(1f);

        GameObject completedBackground = backgrounds[completedIndex];

        // Move the completed background to the end
        for (int i = completedIndex; i < backgrounds.Length - 1; i++)
        {
            backgrounds[i] = backgrounds[i + 1];
        }
        backgrounds[backgrounds.Length - 1] = completedBackground;

        

        //// Increment the move count
        //moveCount++;

        //// Check if moveCount equals the length of the backgrounds array
        //if (moveCount >= backgrounds.Length)
        //{
        //    // Reposition all backgrounds below the current background
        //    float currentBottomPosition = backgrounds[backgrounds.Length - 1].transform.position.y;
        //    for (int i = 0; i < backgrounds.Length; i++)
        //    {
        //        backgrounds[i].transform.position = new Vector2(0, currentBottomPosition - (i + 1) * backgroundHeight);
        //    }
        //    //Adding Check to Adjust the current variable
        //    check = check + backgrounds.Length;

        //    // Reset moveCount
        //    moveCount = 0;
        //}
        //else
        //{
            // Calculate the new position for the completed background
            Vector2 newPosition = new Vector2(0, -(backgrounds.Length + check) * backgroundHeight);
            // Move the completed background to the new position
            completedBackground.transform.position = newPosition;

            check++;
       // }
    }
}
