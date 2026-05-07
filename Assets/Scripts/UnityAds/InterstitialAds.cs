using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string androidUnitID = "Interstitial_Android";
    public static event Action OnAdClosed;
    public void LoadInterstitialAd()
    {
        Advertisement.Load(androidUnitID, this);
    }
    public void ShowInterstitialAd()
    {
        Advertisement.Show(androidUnitID, this);
    }
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial Ad loaded");
        
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message){
        Debug.Log("Ошибка загрузки межстраничной рекламы ");
    }

    public void OnUnityAdsShowClick(string placementId){  }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Time.timeScale = 1f;
        OnAdClosed?.Invoke();
        LoadInterstitialAd();
        AdsManager.Instance.lastAdShowTime = Time.realtimeSinceStartup;
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message){
        Debug.LogError($"Ошибка показа: {message}");
        Time.timeScale = 1f;
        LoadInterstitialAd(); 
    }
    public void OnUnityAdsShowStart(string placementId)
    {
        Time.timeScale = 0f;
        Debug.Log("Реклама началась, ставим игру на паузу");
    }
}
