using System;
using UnityEngine.Advertisements;
using UnityEngine;


public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager instance;

    [Header("REMOVE ADS")] public bool removeAds;
    private Action rewardSuccessAction;
    public string GAME_ID = "3003911";
    private const string REMOVE_ADS = "Remove_Ads";
    [SerializeField] string BANNER_PLACEMENT = "Banner_Android";
    [SerializeField] string VIDEO_PLACEMENT = "Interstitial_Android";
    [SerializeField] string REWARDED_VIDEO_PLACEMENT = "Rewarded_Android";
    [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;
    public bool testMode = true;
    private bool isRewardedAdLoaded = false;
    private bool isNonRewardedAdLoaded = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (removeAds)
            RemoveAds(removeAds);
        Initialize();
    }
    #region Remove Ads
    public bool CanShowAds()
    {
        if (!PlayerPrefs.HasKey(REMOVE_ADS))
            return true;
        else if (PlayerPrefs.GetInt(REMOVE_ADS) == 0)
            return true;
        return false;
    }
    public void RemoveAds(bool remove)
    {
        if (remove)
            PlayerPrefs.SetInt(REMOVE_ADS, 1);
        else
            PlayerPrefs.SetInt(REMOVE_ADS, 0);
    }
    #endregion
    public void Initialize()
    {
        Advertisement.Initialize(GAME_ID, testMode, this);
    }
    public bool IsRewardedAdReady()
    {
        return isRewardedAdLoaded; // Return the flag indicating loaded state
    }
    public bool IsNonRewardedAdReady()
    {
        return isNonRewardedAdLoaded; // Return the flag indicating loaded state
    }
    public void LoadRewardedAd()
    {
        isRewardedAdLoaded = false;
        Advertisement.Load(REWARDED_VIDEO_PLACEMENT, this);
    }
    public void ShowRewardedAd(Action rewardFunction)
    {
        if (isRewardedAdLoaded)
        {
            rewardSuccessAction = null;
            rewardSuccessAction = rewardFunction;
            Advertisement.Show(REWARDED_VIDEO_PLACEMENT, this);
        }
        else
        {
            LoadRewardedAd();
        }
    }
    public void LoadNonRewardedAd()
    {
        if (!CanShowAds())
            return;
        isNonRewardedAdLoaded = false;
        Advertisement.Load(VIDEO_PLACEMENT, this);
    }
    public void ShowNonRewardedAd()
    {
        if (!CanShowAds())
            return;
        if (isNonRewardedAdLoaded)
        {
            Advertisement.Show(VIDEO_PLACEMENT, this);
        }
        else
        {
            LoadNonRewardedAd();
        }
    }
    public void LoadBanner()
    {
        Advertisement.Banner.SetPosition(bannerPosition);
        DestroyBanner();
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
        Advertisement.Banner.Load(BANNER_PLACEMENT, options);
    }
    public void ShowBanner()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };
        Advertisement.Banner.Show(BANNER_PLACEMENT, options);
    }
    public void HideBanner()
    {
        Advertisement.Banner.Hide(false);
    }
    public void DestroyBanner()
    {
        Advertisement.Banner.Hide(true);
    }

    #region Interface Implementations
    public void OnInitializationComplete()
    {
        Debug.Log("Init Success");
        LoadNonRewardedAd();
        LoadRewardedAd();

    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Init Failed: [{error}]: {message}");
    }
    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == REWARDED_VIDEO_PLACEMENT)
        {
            isRewardedAdLoaded = true;
        }
        else if (placementId == VIDEO_PLACEMENT)
        {
            isNonRewardedAdLoaded = true;
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
        if (placementId == VIDEO_PLACEMENT)
        {
            LoadNonRewardedAd();
        }
        else if (placementId == REWARDED_VIDEO_PLACEMENT)
        {
            LoadRewardedAd();
        }
        if (placementId.Equals(REWARDED_VIDEO_PLACEMENT) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            if (rewardSuccessAction != null)
            {
                rewardSuccessAction.Invoke();
            }
        }
    }
    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
        ShowBanner();
    }
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
    }
    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }

    #endregion

}
