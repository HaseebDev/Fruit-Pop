using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FruitManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Fruit[] fruitPrefabs;
    [SerializeField] private BombManager bombPrefab;
    [SerializeField] private Fruit[] spawnableFruits;
    [SerializeField] private Transform fruitsParent;
    [SerializeField] private LineRenderer fruitSpawnLine;
    [SerializeField] private Transform fruitSpawnObject;
    [SerializeField] private BackgroundManager backgroundManager; // Added BackgroundManager reference
    [SerializeField] private ParticleSystem explosionEffect; // Explosion effect for the end of target cycle
    private Fruit currentFruit;
    [SerializeField] private Image targetFruitImage;
    [Header(" Settings ")]
    [SerializeField] private float fruitsYSpawnPos;
    [SerializeField] private Transform fruitsYSpawnPosT;
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
    [SerializeField] private SpriteRenderer[] targetlock;

    // Add a boolean variable to track if the power-up mode is active
    private bool isPowerUpActive = false;
    public bool isPowerUp2Active = false;

    // Reference to the power-up button in the UI
    [SerializeField] private Button powerUpButton;
    [SerializeField] private Button powerUp2Button;
    private BombManager currentBomb; // Variable to track the current bomb being controlled
    private bool isBombControlling; // Flag to indicate if a bomb is currently being controlled
    private bool bombSpawnedThisMouseDown = false; // Add a new flag to track bomb spawning

    public bool TargetFruitAniamtion = false;

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
        // Add listener to the power-up button
        if (powerUpButton != null)
        {
            powerUpButton.onClick.AddListener(ActivatePowerUp1);
        }
        if (powerUp2Button != null)
        {
            powerUp2Button.onClick.AddListener(ActivatePowerUp2);

        }
    }

    void Update()
    {
        // Check if the game state is active and if TargetFruitAnimation is false
        if (!GameManager.instance.IsGameState() || TargetFruitAniamtion)
            return;

        // Check if there are any fruits spawned
        bool fruitsSpawned = fruitsParent.childCount > 0;

        if (powerUpButton != null && powerUp2Button != null)
        {
            powerUpButton.interactable = fruitsSpawned && !isPowerUpActive && !isPowerUp2Active && !TargetFruitAniamtion;
            powerUp2Button.interactable = fruitsSpawned && !isPowerUpActive && !isPowerUp2Active && !TargetFruitAniamtion;
        }

        if (canControl)
            ManagePlayerInput();


        if (isPowerUpActive && Input.GetMouseButtonDown(0))
        {
            // Check if the power-up mode is active and player clicked
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                Fruit clickedFruit = hit.collider.GetComponent<Fruit>();
                if (clickedFruit != null)
                {
                    // Destroy the clicked fruit
                    Destroy(clickedFruit.gameObject);
                    // Deactivate the power-up instantly

                    if (i == 0)
                    {
                        Invoke(nameof(DelayedBoolienTurnout), 2f);
                        i++;
                    }
                    Debug.Log("Power-up deactivated!");
                }
            }
        }

    }
    int i = 0;
    private void DelayedBoolienTurnout()
    {
        isPowerUpActive = false;
        i = 0;
    }

    private void ManagePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
            MouseDownCallback();
        else if (Input.GetMouseButton(0))
        {
            if (isControlling || isBombControlling)
                MouseDragCallback();
            else
                MouseDownCallback();
        }
        else if (Input.GetMouseButtonUp(0) && isControlling || isBombControlling)
            MouseUpCallback();
    }

    private void MouseDownCallback()
    {
        if (!isPowerUpActive && !EventSystem.current.IsPointerOverGameObject())
        {
            if (!isPowerUp2Active && !isBombControlling) // Check if bomb power-up is not active and no bomb is currently being controlled
            {
                DisplayLine();
                PlaceLineAtClickedPosition();
                SpawnFruit(); // Spawn a fruit
                isControlling = true;
            }
            else if (isPowerUp2Active && !isBombControlling) // Check if bomb power-up is active and no bomb is currently being controlled
            {
                DisplayLine();
                PlaceLineAtClickedPosition();
                SpawnBomb(); // Spawn a bomb
                isBombControlling = true;
            }
        }
    }





    private void MouseDragCallback()
    {
        if (isBombControlling)
        {
            // Place line at clicked position for bomb
            PlaceLineAtClickedPosition();
            // Move the bomb to the new position
            currentBomb.MoveTo(new Vector2(GetSpawnPosition().x, fruitsYSpawnPosT.transform.position.y));
        }
        else
        {
            // Place line at clicked position for fruit
            PlaceLineAtClickedPosition();
            // Move the current fruit
            currentFruit.MoveTo(new Vector2(GetSpawnPosition().x, fruitsYSpawnPosT.transform.position.y));
        }
    }


    private void MouseUpCallback()
    {
        HideLine();
        if (isBombControlling)
        {
            if (currentBomb != null)
            {

                if (currentBomb != null)
                {
                    currentBomb.EnablePhysics();
                }
            }
            // Stop controlling the bomb
            StopBombControl();
        }
        else
        {
            // Stop controlling the fruit
            if (currentFruit != null)
                currentFruit.EnablePhysics();
        }
        canControl = false;
        StartControlTimer();
        isControlling = false;
    }

    private void SpawnFruit()
    {

        Vector2 spawnPosition = GetSpawnPosition();
        Fruit fruitToInstantiate = spawnableFruits[nextFruitIndex];
        currentFruit = Instantiate(fruitToInstantiate, spawnPosition, Quaternion.identity, fruitsParent);
        //Debug.Log(" " + currentFruit.transform.parent.GetChild(0).transform.localScale);
        SetNextFruitIndex();


    }

    private void SetNextFruitIndex()
    {
        float commonChance = 0.6f; // Chance for the common fruits (index 0 and index 1)
        float rareChance = 0.3f;   // Chance for the rare fruit (index 2)
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
        worldClickedPosition.y = fruitsYSpawnPosT.position.y;
        return worldClickedPosition;
    }

    private void PlaceLineAtClickedPosition()
    {
        fruitSpawnLine.SetPosition(0, GetSpawnPosition());
        Vector2 Temp = GetSpawnPosition();
        Temp.y = Temp.y + 0.55f;
        fruitSpawnObject.position = Temp;
        fruitSpawnLine.SetPosition(1, GetSpawnPosition() + Vector2.down * 6);
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
        if (TargetFruitAniamtion)
            return;

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
            backgroundManager.MoveToNextLevel();
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
        Invoke(nameof(UpdateTargetFruitImage), 4f);  // Update the target image
        //UpdateTargetFruitImage();
    }

    private IEnumerator CompleteCycle()
    {
        yield return new WaitForSeconds(4.5f);
        // Play explosion effect
        explosionEffect.Play();

        // Wait for the explosion effect to finish
        //yield return new WaitForSeconds(explosionEffect.main.duration);

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
        UpdateTargetFruitImage();
    }


    private int CalculateXpForMerge(FruitType fruitType)
    {
        // Adjust the XP values based on the fruit type
        return (int)Mathf.Pow(2, (int)fruitType) * 2; // Example calculation, you can adjust as needed
    }
    private void SetInitialTargetFruitImage()
    {
        if (targetFruitImage != null && fruitPrefabs.Length > targetFruitIndex)
        {
            foreach (SpriteRenderer target in targetlock)
            {
                target.sprite = fruitPrefabs[targetFruitIndex].GetSprite();
            }

            targetFruitImage.sprite = fruitPrefabs[targetFruitIndex].GetSprite();
        }
    }
    private void UpdateTargetFruitImage()
    {
        if (targetFruitImage != null && fruitPrefabs.Length > targetFruitIndex)
        {
            foreach (SpriteRenderer target in targetlock)
            {
                target.sprite = fruitPrefabs[targetFruitIndex].GetSprite();
            }


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
    // Method to activate the power-up1 mode
    private void ActivatePowerUp1()
    {
        isPowerUpActive = true;
        // You can add visual feedback or any other effects to indicate the power-up mode is active
        Debug.Log("Power-up activated!");
        //Show Rewarded
    }
    private void ActivatePowerUp2()
    {
        isPowerUp2Active = true;
        Debug.Log("Power-up activated!");
        //Show Rewarded
    }
    private void SpawnBomb()
    {
        Vector2 spawnPosition = GetSpawnPosition();
        BombManager bombToInstantiate = bombPrefab;
        currentBomb = Instantiate(bombToInstantiate, spawnPosition, Quaternion.identity);
        isBombControlling = true; // Set flag to indicate bomb is being controlled
    }

    private void StopBombControl()
    {
        isBombControlling = false; // Reset flag
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!enableGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-50, fruitsYSpawnPosT.position.y, 0), new Vector3(50, fruitsYSpawnPosT.position.y, 0));
    }
#endif
}
