using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BucketManager : MonoBehaviour
{
    [SerializeField] private Image bucketFiller;
    [SerializeField] private ParticleSystem particleEffect;
    [SerializeField] private float maxFillAmount = 1.0f;

    private float currentFillAmount = 0.0f;

    private string fillerAmountKey = "FillerAmount";
    [SerializeField] private AudioSource audioS;
    // Method to add to the filler amount
    public void AddToFiller(float amountToAdd)
    {
        currentFillAmount += amountToAdd;

        // Check if filler is filled
        if (currentFillAmount >= maxFillAmount)
        {
            audioS.Play();
            // Trigger particle effect
            if (particleEffect != null)
            {
                particleEffect.Play();
            }

            //Spawn Bucket and Play Animation
            FruitManager.Instance.SpawnBucket();


            ResetFiller();
        }

        // Clamp fill amount to prevent overflow
        currentFillAmount = Mathf.Clamp(currentFillAmount, 0.0f, maxFillAmount);

        // Update UI or any other visual representation of fill amount
        UpdateFillerVisual();

        // Save filler amount
        SaveFillerAmount();
    }

    // Method to update the UI or any visual representation of fill amount
    private void UpdateFillerVisual()
    {
        if (bucketFiller != null)
        {
            // Update the image fill amount
            bucketFiller.fillAmount = currentFillAmount / maxFillAmount;
        }
    }

    // Method to reset the filler amount
    public void ResetFiller()
    {
        currentFillAmount = 0.0f;
        UpdateFillerVisual();
        SaveFillerAmount();
    }

    // Method to save filler amount using PlayerPrefs
    private void SaveFillerAmount()
    {
        PlayerPrefs.SetFloat(fillerAmountKey, currentFillAmount);
        PlayerPrefs.Save();
    }

    // Method to load filler amount from PlayerPrefs
    private void LoadFillerAmount()
    {
        if (PlayerPrefs.HasKey(fillerAmountKey))
        {
            currentFillAmount = PlayerPrefs.GetFloat(fillerAmountKey);
            UpdateFillerVisual();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load filler amount
        if(SceneManager.GetActiveScene().buildIndex != 5)
        LoadFillerAmount();
        else
        {
            bucketFiller.gameObject.transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
