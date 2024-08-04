using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using System;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager instance;
    [Header("Is Test Ads")]
    public bool isTestAds;

    [Header("Unity Ads")]
    string GAME_ID = "3003911";

    [Header("Android Ids")]
    string ANDROID_GAME_ID = "5622099";
    string ANDROID_INTERSTITIAL_PLACEMENT = "Interstitial_Android";
    string ANDROID_REWARDED_PLACEMENT = "Rewarded_Android";

    [Header("IOS Ids")]
    string IOS_GAME_ID = "5622098";
    string IOS_INTERSTITIAL_PLACEMENT = "Interstitial_iOS";
    string IOS_REWARDED_PLACEMENT = "Rewarded_iOS";

    string INTERSTITIAL_PLACEMENT = "Android_Interstitial";
    string REWARDED_PLACEMENT = "rewardedVideo";

    private bool isUnityRewardedAdLoaded = false;
    private bool isUnityInterstitialLoaded = false;

    Action successAction;
    public bool isInitialize;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        if (isTestAds)
        {
            GAME_ID = ANDROID_GAME_ID;
            INTERSTITIAL_PLACEMENT = ANDROID_INTERSTITIAL_PLACEMENT;
            REWARDED_PLACEMENT = ANDROID_REWARDED_PLACEMENT;
        }
        else
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                GAME_ID = ANDROID_GAME_ID;
                INTERSTITIAL_PLACEMENT = ANDROID_INTERSTITIAL_PLACEMENT;
                REWARDED_PLACEMENT = ANDROID_REWARDED_PLACEMENT;
            }
            else
            {
                GAME_ID = IOS_GAME_ID;
                INTERSTITIAL_PLACEMENT = IOS_INTERSTITIAL_PLACEMENT;
                REWARDED_PLACEMENT = IOS_REWARDED_PLACEMENT;
            }
        }
    }

    void Start()
    {
        Advertisement.Initialize(GAME_ID, isTestAds, this);
    }

    public void ShowInterstitialAd()
    {
        ShowUnityInterstitial();
    }

    public void ShowRewardedAd(Action rewardFunction)
    {
        successAction = rewardFunction;
        ShowUnityRewardedAd();
    }

    public bool IsNonRewardedAdReady()
    {
        return isUnityInterstitialLoaded; 
    }

    public void LoadUnityRewardedAd()
    {
        isUnityRewardedAdLoaded = false;
        Advertisement.Load(REWARDED_PLACEMENT, this);
    }

    public void ShowUnityRewardedAd()
    {
        if (isUnityRewardedAdLoaded)
        {
            Advertisement.Show(REWARDED_PLACEMENT, this);
        }
        else
        {
            LoadUnityRewardedAd();
        }
    }

    public void LoadUnityInterstitial()
    {
        isUnityInterstitialLoaded = false;
        Advertisement.Load(INTERSTITIAL_PLACEMENT, this);
    }

    public void ShowUnityInterstitial()
    {
        if (isUnityInterstitialLoaded)
        {
            Advertisement.Show(INTERSTITIAL_PLACEMENT, this);
        }
        else
        {
            LoadUnityInterstitial();
        }
    }

    #region Interface Implementations

    public void OnInitializationComplete()
    {
        Debug.Log("Init Success");
        LoadUnityInterstitial();
        LoadUnityRewardedAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Init Failed: [{error}]: {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == REWARDED_PLACEMENT)
        {
            isUnityRewardedAdLoaded = true;
        }
        else if (placementId == INTERSTITIAL_PLACEMENT)
        {
            isUnityInterstitialLoaded = true;
        }
        Debug.Log($"Load Success: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Load Failed: [{error}:{placementId}] {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"OnUnityAdsShowFailure: [{error}]: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"OnUnityAdsShowStart: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"OnUnityAdsShowClick: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"OnUnityAdsShowComplete: [{showCompletionState}]: {placementId}");
        if (placementId == INTERSTITIAL_PLACEMENT)
        {
            LoadUnityInterstitial();
        }
        else if (placementId == REWARDED_PLACEMENT)
        {
            LoadUnityRewardedAd();
        }
        if (placementId.Equals(REWARDED_PLACEMENT) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            successAction?.Invoke();
        }
    }

    #endregion
}
