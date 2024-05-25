using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Advertisements;
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
    string IOS_GAME_ID = "5615721";
    string IOS_INTERSTITIAL_PLACEMENT = "Interstitial_iOS";
    string IOS_REWARDED_PLACEMENT = "Rewarded_iOS";

    string INTERSTITIAL_PLACEMENT = "Android_Interstitial";
    string REWARDED_PLACEMENT = "rewardedVideo";

    private bool isUnityRewardedAdLoaded = false;
    private bool isUnityInterstitialLoaded = false;
    [Header("Admob Ads")]

    [Header("Android Ids")]
    private string _androidBannerAdUnitId = "ca-app-pub-5824735341440098/2054640391";
    private string _androidInterAdUnitId = "ca-app-pub-5824735341440098/4788614948";
    private string _androidRewardedAdUnitId = "ca-app-pub-5824735341440098/5016480113";

    [Header("IOS Ids")]
    private string _iosBannerAdUnitId = "ca-app-pub-5824735341440098/3214723568";
    private string _iosInterAdUnitId = "ca-app-pub-5824735341440098/8580754407";
    private string _iosRewardedAdUnitId = "ca-app-pub-5824735341440098/9646940984";

    private string _bannerAdUnitId = "ca-app-pub-5824735341440098/3214723568";
    private string _interAdUnitId = "ca-app-pub-5824735341440098/8580754407";
    private string _rewardedAdUnitId = "ca-app-pub-5824735341440098/9646940984";

    Action successAction;
    BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    bool AdmobRewardedLoaded;
    bool AdmobInterdLoaded;
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
            _interAdUnitId = "ca-app-pub-3940256099942544/1033173712";
            _bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
            _rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
            GAME_ID = ANDROID_GAME_ID;
            INTERSTITIAL_PLACEMENT = ANDROID_INTERSTITIAL_PLACEMENT;
            REWARDED_PLACEMENT = ANDROID_REWARDED_PLACEMENT;
        }
        else
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                _interAdUnitId = _androidInterAdUnitId;
                _bannerAdUnitId = _androidBannerAdUnitId;
                _rewardedAdUnitId = _androidRewardedAdUnitId;
                GAME_ID = ANDROID_GAME_ID;
                INTERSTITIAL_PLACEMENT = ANDROID_INTERSTITIAL_PLACEMENT;
                REWARDED_PLACEMENT = ANDROID_REWARDED_PLACEMENT;
            }
            else
            {
                _interAdUnitId = _iosInterAdUnitId;
                _bannerAdUnitId = _iosBannerAdUnitId;
                _rewardedAdUnitId = _iosRewardedAdUnitId;
                GAME_ID = IOS_GAME_ID;
                INTERSTITIAL_PLACEMENT = IOS_INTERSTITIAL_PLACEMENT;
                REWARDED_PLACEMENT = IOS_REWARDED_PLACEMENT;
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            LoadAdmobInterstitialAd(); LoadAdmobRewardedAd();
        });

        Advertisement.Initialize(GAME_ID, isTestAds, this);
    }

    #region Banner
    public void CreateAdmobBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAdmobBannerAd();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
    }
    public void LoadAdmobBannerAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateAdmobBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    public void HideAdmobBanner()
    {
        _bannerView.Hide();
    }
    private void DestroyAdmobBannerAd()
    {
        _bannerView.Destroy();
        _bannerView = null;

    }
    #region Banner Listener Events
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }
    #endregion

    #endregion Banner

    #region Interstitial

    public void LoadAdmobInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_interAdUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    AdmobInterdLoaded = false;
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());
                AdmobInterdLoaded = true;


                _interstitialAd = ad;
            });
    }

    public void ShowAdmobInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
    }


    #region Interstitial Listerner Events
    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };

        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadAdmobInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadAdmobInterstitialAd();
        };


    }

    #endregion


    #endregion

    #region Rewarded


    public void LoadAdmobRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_rewardedAdUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    AdmobRewardedLoaded = false;
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());
                AdmobRewardedLoaded = true;


                _rewardedAd = ad;
            });
    }

    public void ShowAdmobRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((GoogleMobileAds.Api.Reward reward) =>
            {
                successAction?.Invoke();
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
        else
        {
            ShowUnityRewardedAd();
        }
    }

    public bool CheckRewardedAvailable()
    {
        if (_rewardedAd.CanShowAd() || isUnityRewardedAdLoaded)
            return true;
        else
            return false;
    }


    #region Rewarded Listerner Events
    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadAdmobRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadAdmobRewardedAd();
        };


    }

    #endregion

    #endregion




    public void ShowInterstitialAd()
    {

        if (AdmobInterdLoaded)
        {
            if (_interstitialAd.CanShowAd())
            {
                ShowAdmobInterstitialAd();
            }
            else
            {
                ShowUnityInterstitial();
                LoadAdmobInterstitialAd();

            }
        }

        else
        {
            ShowUnityInterstitial();
            LoadAdmobInterstitialAd();

        }

    }
    public void ShowRewardedAd(Action rewardFunction)
    {

        successAction = rewardFunction;
        if (AdmobRewardedLoaded)
        {
            if (_rewardedAd.CanShowAd())
            {
                ShowAdmobRewardedAd();
            }
            else
            {
                ShowUnityRewardedAd();
                LoadAdmobRewardedAd();

            }
        }
        else
        {
            ShowUnityRewardedAd();
            LoadAdmobRewardedAd();

        }

    }


    public bool IsNonRewardedAdReady()
    {
        return isUnityInterstitialLoaded; // Return the flag indicating loaded state
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