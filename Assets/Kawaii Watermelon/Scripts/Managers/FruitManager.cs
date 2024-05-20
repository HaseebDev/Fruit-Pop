using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;
using UnityEngine.UI;

public class FruitManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Fruit[] fruitPrefabs;
    [SerializeField] private Fruit[] spawnableFruits;
    [SerializeField] private Transform fruitsParent;
    [SerializeField] private LineRenderer fruitSpawnLine;
    [SerializeField] private BackgroundManager backgroundManager; // Added BackgroundManager reference
    [SerializeField] private ParticleSystem explosionEffect; // Explosion effect for the end of target cycle
    private Fruit currentFruit;
    [SerializeField] private Image targetFruitImage;
    [Header(" Settings ")]
    [SerializeField] private float fruitsYSpawnPos;
    [SerializeField] private float spawnDelay;
    private bool canControl;
    private bool isControlling;

    [Header(" Next Fruit Settings ")]
    private int nextFruitIndex;
    private int targetFruitIndex = 5; // Starting target fruit index

    [Header(" Debug ")]
    [SerializeField] private bool enableGizmos;

    [Header(" Actions ")]
    public static Action onNextFruitIndexSet;

    [Header(" Level Manager ")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private float fallSpeed = 5.0f;
    private void Awake()
    {
        MergeManager.onMergeProcessed += MergeProcessedCallback;
    }

    private void OnDestroy()
    {
        MergeManager.onMergeProcessed -= MergeProcessedCallback;
    }

    void Start()
    {
        SetInitialTargetFruitImage();
        SetNextFruitIndex();
        canControl = true;
        HideLine();
    }

    void Update()
    {
        if (!GameManager.instance.IsGameState())
            return;

        if (canControl)
            ManagePlayerInput();
    }

    private void ManagePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
            MouseDownCallback();
        else if (Input.GetMouseButton(0))
        {
            if (isControlling)
                MouseDragCallback();
            else
                MouseDownCallback();
        }
        else if (Input.GetMouseButtonUp(0) && isControlling)
            MouseUpCallback();
    }

    private void MouseDownCallback()
    {
        DisplayLine();
        PlaceLineAtClickedPosition();
        SpawnFruit();
        isControlling = true;
    }

    private void MouseDragCallback()
    {
        PlaceLineAtClickedPosition();
        currentFruit.MoveTo(new Vector2(GetSpawnPosition().x, fruitsYSpawnPos));
    }

    private void MouseUpCallback()
    {
        HideLine();
        if (currentFruit != null)
            currentFruit.EnablePhysics();
        canControl = false;
        StartControlTimer();
        isControlling = false;
    }

    private void SpawnFruit()
    {
        Vector2 spawnPosition = GetSpawnPosition();
        Fruit fruitToInstantiate = spawnableFruits[nextFruitIndex];
        currentFruit = Instantiate(fruitToInstantiate, spawnPosition, Quaternion.identity, fruitsParent);
        SetNextFruitIndex();
    }

    private void SetNextFruitIndex()
    {
        float commonChance = 0.6f; // Chance for the common fruits (index 0 and index 1)
        float rareChance = 0.1f;   // Chance for the rare fruit (index 2)
        float superRareChance = 0.5f; // Chance for the super rare fruit (index 3)

        float randomValue = Random.value;
        if (randomValue < commonChance)
        {
            nextFruitIndex = Random.Range(0, 2); // Randomly choose between common fruits
            Debug.Log("Common fruit selected.");
        }
        else if (randomValue < commonChance + rareChance)
        {
            nextFruitIndex = 2; // Rare fruit index
            Debug.Log("Rare fruit selected.");
        }
        else if (randomValue < commonChance + rareChance + superRareChance)
        {
            nextFruitIndex = 3; // Super rare fruit index
            Debug.Log("Super rare fruit selected.");
        }
        else
        {
            nextFruitIndex = Random.Range(0, 2); // Default to common fruit index if none of the above conditions are met
            Debug.Log("Defaulting to common fruit.");
        }

        // Prevent the target fruit from spawning
        while (nextFruitIndex == targetFruitIndex)
        {
            nextFruitIndex = Random.Range(0, 4); // Randomly choose any index
        }

        onNextFruitIndexSet?.Invoke();
    }





    public string GetNextFruitName()
    {
        return spawnableFruits[nextFruitIndex].name;
    }

    public Sprite GetNextFruitSprite()
    {
        return spawnableFruits[nextFruitIndex].GetSprite();
    }

    private Vector2 GetClickedWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 worldClickedPosition = GetClickedWorldPosition();
        worldClickedPosition.y = fruitsYSpawnPos;
        return worldClickedPosition;
    }

    private void PlaceLineAtClickedPosition()
    {
        fruitSpawnLine.SetPosition(0, GetSpawnPosition());
        fruitSpawnLine.SetPosition(1, GetSpawnPosition() + Vector2.down * 15);
    }

    private void HideLine()
    {
        fruitSpawnLine.enabled = false;
    }

    private void DisplayLine()
    {
        fruitSpawnLine.enabled = true;
    }

    private void StartControlTimer()
    {
        Invoke("StopControlTimer", spawnDelay);
    }

    private void StopControlTimer()
    {
        canControl = true;
    }

    private void MergeProcessedCallback(FruitType fruitType, Vector2 spawnPosition)
    {
        for (int i = 0; i < fruitPrefabs.Length; i++)
        {
            if (fruitPrefabs[i].GetFruitType() == fruitType)
            {
                SpawnMergedFruit(fruitPrefabs[i], spawnPosition);
                break;
            }
        }

        int xp = CalculateXpForMerge(fruitType);
        levelManager.AddXp(xp);

        // Check if target fruit is achieved
        if ((int)fruitType == targetFruitIndex)
        {
            TargetAchieved();
        }
    }

    private void SpawnMergedFruit(Fruit fruit, Vector2 spawnPosition)
    {
        Fruit fruitInstance = Instantiate(fruit, spawnPosition, Quaternion.identity, fruitsParent);
        fruitInstance.EnablePhysics();
    }

    private void TargetAchieved()
    {

        //Make Fruits Fall Through the current playing backgrounds gate(Still needs to figure out how i am going to do that) 
        //Play the AchievedTarget Fruit breaking the gate and at the end of the animation switch call the backgroundManager.MoveToNextLevel();


        // Find the target fruit in the scene
        Fruit targetFruit = null;
        foreach (Transform fruitTransform in fruitsParent)
        {
            Fruit fruit = fruitTransform.GetComponent<Fruit>();
            if (fruit != null && (int)fruit.GetFruitType() == targetFruitIndex)
            {
                targetFruit = fruit;
                break;
            }
        }

        if (targetFruit != null)
        {
            // Add the TargetFruitAnimation component and start the animation
            TargetFruitAnimation targetFruitAnimation = targetFruit.gameObject.AddComponent<TargetFruitAnimation>();
            targetFruitAnimation.StartTargetFruitAnimation();
            //Disable the Backgrunds Gates Collider. 
            //Make all other Fruit Follow //MeanWhile Also do backgroundManager.MoveToNextLevel(); It should be done above with almost the matching time delay.
        }

        if (targetFruitIndex < fruitPrefabs.Length - 1)
        {
            targetFruitIndex++;
        }
        else
        {
            StartCoroutine(CompleteCycle());
        }

        UpdateTargetFruitImage(); // Update the target image
    }

    private IEnumerator CompleteCycle()
    {
        // Play explosion effect
        explosionEffect.Play();

        // Wait for the explosion effect to finish
        yield return new WaitForSeconds(explosionEffect.main.duration);

        // Add XP to the level
        int explosionXp = 1000; // Adjust as needed
        levelManager.AddXp(explosionXp);

        // Destroy all fruits
        foreach (Transform fruitTransform in fruitsParent)
        {
            Destroy(fruitTransform.gameObject);
        }

        // Reset target fruit index to element 5
        targetFruitIndex = 5;
    }


    private int CalculateXpForMerge(FruitType fruitType)
    {
        // Adjust the XP values based on the fruit type
        return (int)Mathf.Pow(2, (int)fruitType); // Example calculation, you can adjust as needed
    }
    private void SetInitialTargetFruitImage()
    {
        if (targetFruitImage != null && fruitPrefabs.Length > targetFruitIndex)
        {
            targetFruitImage.sprite = fruitPrefabs[targetFruitIndex].GetSprite();
        }
    }
    private void UpdateTargetFruitImage()
    {
        if (targetFruitImage != null && fruitPrefabs.Length > targetFruitIndex)
        {
            targetFruitImage.sprite = fruitPrefabs[targetFruitIndex].GetSprite();
        }
    }


    public void MakeFruitsFall(Transform gatePosition, float collectDuration, float fallDuration)
    {
        StartCoroutine(CollectAndFallFruitsCoroutine(gatePosition, collectDuration, fallDuration));
    }

    private IEnumerator CollectAndFallFruitsCoroutine(Transform gatePosition, float collectDuration, float fallDuration)
    {
        // Get the initial position of the fruits parent
        Vector3 initialFruitsParentPosition = fruitsParent.position;

        // Calculate the final position for collecting the fruits in the middle
        Vector3 collectPosition = (gatePosition.position + initialFruitsParentPosition) / 2;

        // Smoothly move the fruits parent to the collect position
        float elapsedTime = 0f;
        while (elapsedTime < collectDuration)
        {
            fruitsParent.position = Vector3.Lerp(initialFruitsParentPosition, collectPosition, elapsedTime / collectDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the fruits parent reaches the collect position
        fruitsParent.position = collectPosition;

        // Wait for a short duration before making the fruits fall
        yield return new WaitForSeconds(0.5f);

        // Calculate the direction for fruit falling
        Vector3 fallDirection = (gatePosition.position - collectPosition).normalized;

        // Fall all fruits towards the gate position
        foreach (Transform fruit in fruitsParent)
        {
            Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = fallDirection * fallSpeed;
            }
        }

        // Wait for the specified fall duration before completing the transition
        yield return new WaitForSeconds(fallDuration);

        // Trigger the completion of the transition or any other desired action here
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!enableGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-50, fruitsYSpawnPos, 0), new Vector3(50, fruitsYSpawnPos, 0));
    }
#endif
}
