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

    private Camera mainCamera;
    private Vector3 initialCameraPosition;

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
            StartCoroutine(AnimateTargetFruit());
        }
    }

    private IEnumerator AnimateTargetFruit()
    {
        isAnimating = true;

        Vector3 centerScreenPosition = new Vector3(0, 0, originalPosition.z);
        fruitRigidbody.isKinematic = true;
        while (Vector3.Distance(fruitTransform.position, centerScreenPosition) > 0.1f)
        {
            fruitTransform.position = Vector3.MoveTowards(fruitTransform.position, centerScreenPosition, liftSpeed * Time.deltaTime);
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
        while (Vector3.Distance(fruitTransform.position, bottomCenterPosition) > 0.1f)
        {
            fruitTransform.position = Vector3.MoveTowards(fruitTransform.position, bottomCenterPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }

        isAnimating = false;
        fruitRigidbody.isKinematic = false;

        //Destroy(this);
    }

    private void Update()
    {
        // Follow the fruit only when it's not animating and it's moving downwards
        if (!isAnimating)
        {
            // Calculate the offset between the fruit's current position and its original position
            Vector3 offset = fruitTransform.position - originalPosition;

            // Adjust the camera's y-position based on the offset
            Vector3 newCameraPosition = new Vector3(initialCameraPosition.x, initialCameraPosition.y + offset.y - -0.8f, initialCameraPosition.z);
            mainCamera.transform.position = newCameraPosition;
        }
    }


}
