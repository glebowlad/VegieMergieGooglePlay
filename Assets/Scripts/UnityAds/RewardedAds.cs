using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string androidUnitID = "Rewarded_Android";
    public static event Action OnRewardedAdClosed;
    private Action onRewardGranted;

    private bool isAdLoaded = false;
    public bool IsAdAvailable()
    {
        return isAdLoaded;
    }
    public void RetryLoadAd()
    {
        // Если реклама еще не загружена, запрашиваем её снова
        if (!isAdLoaded)
        {
            Debug.Log("Повторный ручной запрос на загрузку рекламы...");
            LoadRewardedAd();
        }
    }
    public void LoadRewardedAd()
    {
        isAdLoaded = false;
        Advertisement.Load(androidUnitID, this);
    }
    public void ShowRewardedAd(Action rewardCallback)
    {
        onRewardGranted=rewardCallback;
        isAdLoaded = false;
        Advertisement.Show(androidUnitID, this);
       
    }
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Rewarded Ad loaded");
        isAdLoaded = true;
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) {
        isAdLoaded = false;
    }

    public void OnUnityAdsShowClick(string placementId) { }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) 
    {
        if (placementId == androidUnitID)
        {
            isAdLoaded = false;
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Reward granted");

                onRewardGranted?.Invoke();
            }

            onRewardGranted = null; 
            OnRewardedAdClosed?.Invoke();
            AdsManager.Instance.lastAdShowTime = Time.realtimeSinceStartup;
          
            LoadRewardedAd();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) 
    {
        Debug.Log("Ошибка показа рекламы за награду");
        isAdLoaded = false;
        onRewardGranted = null;
        OnRewardedAdClosed?.Invoke();
        LoadRewardedAd();

    }
    public void OnUnityAdsShowStart(string placementId) { }
}

