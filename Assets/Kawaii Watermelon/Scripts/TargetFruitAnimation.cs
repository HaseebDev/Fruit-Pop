using System.Collections;
using UnityEngine;

public class TargetFruitAnimation : MonoBehaviour
{
    private Transform fruitTransform;
    private Rigidbody2D fruitRigidbody;
    private Vector3 originalPosition;
    private bool isAnimating = false;

    [SerializeField] private float liftSpeed = 2.0f;
    [SerializeField] private float shakeAmount = 0.01f;
    [SerializeField] private float shakeIncreaseRate = 0.001f;
    [SerializeField] private float centerWaitTime = 2.0f;
    [SerializeField] private float fallSpeed = 10.0f;
    float targetYPosition;
    private Camera mainCamera;
    private Vector3 initialCameraPosition;
    float smoothness;
    private void Awake()
    {
        fruitTransform = transform;
        fruitRigidbody = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    public void StartTargetFruitAnimation()
    {
        if (!isAnimating)
        {
            originalPosition = fruitTransform.position;
            initialCameraPosition = mainCamera.transform.position; // Store initial camera position
            targetYPosition = initialCameraPosition.y - 10.225f;
            StartCoroutine(AnimateTargetFruit());
        }
    }

    private IEnumerator AnimateTargetFruit()
    {
        isAnimating = true;

        Vector3 centerScreenPosition = new Vector3(0, 0, originalPosition.z);
        fruitRigidbody.isKinematic = true;
        while (Vector2.Distance(fruitTransform.position, centerScreenPosition) > 0.1f)
        {
            Vector3 targetPosition = new Vector3(centerScreenPosition.x, centerScreenPosition.y, originalPosition.z);
            fruitTransform.position = Vector3.MoveTowards(fruitTransform.position, targetPosition, liftSpeed * Time.deltaTime);
            fruitTransform.position += (Vector3)Random.insideUnitCircle * shakeAmount;
            shakeAmount += shakeIncreaseRate * Time.deltaTime;
            yield return null;
        }

        float elapsedTime = 0f;
        while (elapsedTime < centerWaitTime)
        {
            fruitTransform.position = centerScreenPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 bottomCenterPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, originalPosition.z));
        fruitRigidbody.velocity = Vector2.zero;
        while (Vector2.Distance(fruitTransform.position, bottomCenterPosition) > 0.1f)
        {
            Vector3 targetPosition = new Vector3(bottomCenterPosition.x, bottomCenterPosition.y, originalPosition.z);
            fruitTransform.position = Vector3.MoveTowards(fruitTransform.position, targetPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }

        smoothness = 3f;
        isAnimating = false;
        fruitRigidbody.isKinematic = false;

        Destroy(this, 4f);
    }

    private void OnDestroy()
    {
        Debug.Log("TargetDestroyed");
    }

    private void Update()
    {
        // Follow the fruit only when it's not animating
        if (!isAnimating)
        {
            // Get the current camera position
            Vector3 currentCameraPosition = mainCamera.transform.position;

            // Define the target Y position (-10.225)


            // Ensure the camera doesn't move more than -10.225
            if (currentCameraPosition.y > targetYPosition)
            {
                // Smoothly interpolate the camera's Y position towards -10.27
                float newYPosition = Mathf.Lerp(currentCameraPosition.y, targetYPosition, Time.deltaTime * smoothness);

                // Update the camera's position with the new Y value
                mainCamera.transform.position = new Vector3(currentCameraPosition.x, newYPosition, currentCameraPosition.z);
            }
        }
    }


}
