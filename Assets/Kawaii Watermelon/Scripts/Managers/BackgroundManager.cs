using System.Collections;
using UnityEngine;

[System.Serializable]
public class BackgroundGatePair
{
    public Transform background;
    public Transform gate;
}

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private BackgroundGatePair[] backgroundGatePairs;
    [SerializeField] private float transitionDuration = 1.0f;
    [SerializeField] private float transitionDistance = 10.0f; // Adjust as needed

    private Vector3[] originalRelativePositions;
    private int currentBackgroundIndex = 0;

    private void Start()
    {
        // Store original relative positions
        StoreOriginalRelativePositions();
    }

    private void StoreOriginalRelativePositions()
    {
        originalRelativePositions = new Vector3[backgroundGatePairs.Length];
        for (int i = 0; i < backgroundGatePairs.Length; i++)
        {
            originalRelativePositions[i] = backgroundGatePairs[i].background.position - backgroundGatePairs[0].background.position;
        }
    }

    private void Update()
    {
        // Check if the last background is reached
        if (currentBackgroundIndex == backgroundGatePairs.Length - 1)
        {
            StartCoroutine(TransitionBackgrounds());
        }
    }

    private IEnumerator TransitionBackgrounds()
    {
        // Move all backgrounds and gates down
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            for (int i = 0; i < backgroundGatePairs.Length; i++)
            {
                Vector3 targetPosition = backgroundGatePairs[i].background.position - Vector3.up * transitionDistance;
                backgroundGatePairs[i].background.position = Vector3.Lerp(backgroundGatePairs[i].background.position, targetPosition, elapsedTime / transitionDuration);
                backgroundGatePairs[i].gate.position = Vector3.Lerp(backgroundGatePairs[i].gate.position, targetPosition, elapsedTime / transitionDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset backgrounds to maintain original relative positions
        for (int i = 0; i < backgroundGatePairs.Length; i++)
        {
            backgroundGatePairs[i].background.position = backgroundGatePairs[0].background.position + originalRelativePositions[i];
        }
    }
}
