using GoogleMobileAds.Api;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdsManager : Singleton<AdsManager>
{
#if UNITY_EDITOR
    private string _adUnitId = "unused";
    private string _adInterstitialId = "unused";
    private string _adRewardedId = "unused";
#elif UNITY_ANDROID
    private string _adUnitId = "unused";
    private string _adInterstitialId = "ca-app-pub-3601362388931534/2644911410";
    private string _adRewardedId = "ca-app-pub-3601362388931534/2193139799";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3601362388931534/7388335155";
#endif

    BannerView _bannerView;
    InterstitialAd _interstitialAd;
    RewardedAd _rewardedAd;

    [HideInInspector] public UnityEvent rewardEvent;
    [HideInInspector] public UnityEvent interstitialAdClosedEvent;
    public GameObject adsFailPanel;
    private static GameObject persistentAdsFailPanel;

    public bool OlunceReklamGoster;

    private NetworkReachability _lastReachability = NetworkReachability.NotReachable;

    protected override void Awake()
    {
        base.Awake();
        if (adsFailPanel != null)
        {
            if (persistentAdsFailPanel != null && persistentAdsFailPanel != adsFailPanel)
            {
                Destroy(adsFailPanel);
            }
            else
            {
                persistentAdsFailPanel = adsFailPanel;
                DontDestroyOnLoad(adsFailPanel);
            }
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("AdMob SDK Initialized.");
            LoadInterstitialAd();
            LoadRewardedAd();
        });
        _lastReachability = Application.internetReachability;
    }

    private void Update()
    {
        if (_lastReachability != Application.internetReachability)
        {
            Debug.Log("Ağ türü değişti: " + Application.internetReachability);
            _lastReachability = Application.internetReachability;

            if (_lastReachability != NetworkReachability.NotReachable)
            {
                LoadInterstitialAd();
                LoadRewardedAd();
            }
        }
    }

    #region Rewarded Ads

    public void ShowRewardedAd()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("İnternet yok, ödüllü reklam gösterilemiyor.");
            ShowAdsFailPanel();
            return;
        }

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded ad rewarded the user. Type: {reward.Type}, amount: {reward.Amount}.");
            });
        }
        else
        {
            LoadRewardedAd(() => TryShowRewardedAd());
        }
    }

    public void ShowAdsFailPanel()
    {
        if (adsFailPanel.TryGetComponent<AdsFailPanel>(out AdsFailPanel panel))
        {
            panel.ShowFaildPanel();
        }
        else
        {
            Debug.LogWarning("AdsFailPanel component bulunamadı!");
        }
    }

    private void TryShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded ad rewarded the user. Type: {reward.Type}, amount: {reward.Amount}.");
            });
        }
    }

    public void LoadRewardedAd(Action onComplete = null)
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");
        var adRequest = new AdRequest();

        RewardedAd.Load(_adRewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                return;
            }
            _rewardedAd = ad;
            RegisterRewardedAdEventHandlers(_rewardedAd);
            onComplete?.Invoke();
        });
    }

    public void ShowLevelCompleteAds(Action onRewardedAdComplete, Action onNoAdFallback = null)
    {
        if (IsRewardedAdReady())
        {
            rewardEvent.RemoveAllListeners();
            rewardEvent.AddListener(() =>
            {
                Debug.Log("Ödüllü reklam izlendi, devam ediliyor.");
                onRewardedAdComplete?.Invoke();
            });
            ShowRewardedAd();
        }
        else if (IsInterstitialAdReady())
        {
            interstitialAdClosedEvent.RemoveAllListeners();
            interstitialAdClosedEvent.AddListener(() =>
            {
                Debug.Log("Geçiş reklamı izlendi, devam ediliyor.");
                onRewardedAdComplete?.Invoke();
            });
            ShowInterstitialAd();
        }
        else
        {
            Debug.LogWarning("Reklam yok, sahne yeniden yüklenecek.");
            onNoAdFallback?.Invoke();
        }
    }


    private void RegisterRewardedAdEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            rewardEvent?.Invoke();
            LoadRewardedAd();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content: " + error);
            LoadRewardedAd();
        };
    }

    #endregion

    #region Interstitial Ads

    public void ShowInterstitialAd()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("İnternet yok, geçişli reklam gösterilemiyor.");
            ShowAdsFailPanel();
            return;
        }

        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            LoadInterstitialAd(() => TryShowInterstitialAd());
        }
    }

    private void TryShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
    }

    public void LoadInterstitialAd(Action onComplete = null)
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");
        var adRequest = new AdRequest();

        InterstitialAd.Load(_adInterstitialId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error);
                return;
            }
            _interstitialAd = ad;
            RegisterInterstitialAdEventHandlers(_interstitialAd);
            onComplete?.Invoke();
        });
    }

    private void RegisterInterstitialAdEventHandlers(InterstitialAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Interstitial ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            interstitialAdClosedEvent?.Invoke();
            LoadInterstitialAd();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content: " + error);
            LoadInterstitialAd();
        };
    }

    #endregion

    #region Banner Ads

    public void LoadAd()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }
        var adRequest = new AdRequest();
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");
        if (_bannerView != null)
        {
            DestroyAd();
        }
        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);
    }

    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    #endregion

    #region Ad Availability

    public bool IsBannerAdReady()
    {
        return _bannerView != null;
    }

    public bool IsInterstitialAdReady()
    {
        return _interstitialAd != null && _interstitialAd.CanShowAd();
    }

    public bool IsRewardedAdReady()
    {
        return _rewardedAd != null && _rewardedAd.CanShowAd();
    }

    #endregion
}