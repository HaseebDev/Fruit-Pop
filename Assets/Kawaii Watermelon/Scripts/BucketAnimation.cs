using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BucketAnimation : MonoBehaviour
{
    public float scaleDuration = 1f;
    public float tiltDuration = 0.5f;
    public float tiltHoldDuration = 1f;
    public float resetRotationDuration = 0.5f; // New variable for resetting rotation
    public float destroyDelay = 0.5f;

    private bool isAnimating = false;

    public static Transform SpawnLocation;

    void Start()
    {
        SpawnLocation = transform.GetChild(0).GetComponent<Transform>();
        StartAnimation();
    }

    void StartAnimation()
    {
        if (!isAnimating && SceneManager.GetActiveScene().buildIndex != 5)
        {
            isAnimating = true;
            StartCoroutine(AnimateBucket());
        }
    }

    IEnumerator AnimateBucket()
    {
        // Scale up animation
        float timer = 0f;
        while (timer < scaleDuration)
        {
            float scaleFactor = timer / scaleDuration;
            transform.localScale = Vector3.one * scaleFactor;
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;

        // Tilt animation
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0f, 0f, 80f); // Tilt right by 80 degrees
        timer = 0f;
        while (timer < tiltDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, timer / tiltDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Spawn fruits during tilt animation
        for (int i = 0; i < 10; i++)
        {
            FruitManager.Instance.SpawnBucketFruit();
            yield return new WaitForSeconds(0.2f);
        }

        // Hold tilt position
        yield return new WaitForSeconds(tiltHoldDuration);

        // Reset rotation animation
        Quaternion currentRotation = transform.rotation;
        timer = 0f;
        while (timer < resetRotationDuration)
        {
            transform.rotation = Quaternion.Lerp(currentRotation, Quaternion.identity, timer / resetRotationDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.identity; // Ensure rotation is exactly zero

        // Scale down animation
        timer = 0f;
        while (timer < scaleDuration)
        {
            float scaleFactor = 1f - (timer / scaleDuration);
            transform.localScale = Vector3.one * scaleFactor;
            timer += Time.deltaTime;
            yield return null;
        }

        // Destroy bucket after delay
        yield return new WaitForSeconds(destroyDelay);


        isAnimating = false;
        FruitManager.Instance.isBucketActive = false;
        Debug.Log("FruitManager.Instance.isBucketActive " + FruitManager.Instance.isBucketActive);
        Destroy(gameObject);
    }
}
