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

    private void Awake()
    {
        fruitTransform = transform;
        fruitRigidbody = GetComponent<Rigidbody2D>();
    }

    public void StartTargetFruitAnimation()
    {
        if (!isAnimating)
        {
            originalPosition = fruitTransform.position;
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

        Vector3 bottomCenterPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f, originalPosition.z));
        fruitRigidbody.velocity = Vector2.zero;
        while (Vector3.Distance(fruitTransform.position, bottomCenterPosition) > 0.1f)
        {
            fruitTransform.position = Vector3.MoveTowards(fruitTransform.position, bottomCenterPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }

        isAnimating = false;
        fruitRigidbody.isKinematic = false;

        Destroy(this);
    }

  
}
