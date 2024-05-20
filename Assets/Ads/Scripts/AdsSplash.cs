using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AdsSplash : MonoBehaviour
{
    public Slider mySlider;
    public TextMeshProUGUI progressText;
    private float startTime;
    public float totalDuration = 60f;
    public bool loadingStarted = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        float startTime = Time.time;
        float progress = 0f;
        loadingStarted = true;
        while (progress < 1f)
        {
            progress = (Time.time - startTime) / totalDuration;
            mySlider.value = progress;
            progressText.text = "Loading " + Mathf.FloorToInt(progress * 100) + "%";
            yield return new WaitForSeconds(Time.deltaTime);
        }
        LoadingComplete();
    }
    public void StartLoading()
    {
        StartCoroutine("Loading");
    }
    private void LoadingComplete()
    {
        AdsManager.instance.ShowNonRewardedAd();
        Debug.Log("Loading Complete!");
        Invoke(nameof(LoadNextScene), 1);
    }
    void LoadNextScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
