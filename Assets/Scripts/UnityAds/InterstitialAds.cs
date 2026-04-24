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
        Time.timeScale = 0f;
        Advertisement.Show(androidUnitID, this);
    }
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial Ad loaded");
        
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message){  }

    public void OnUnityAdsShowClick(string placementId){  }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Time.timeScale = 1f;
        OnAdClosed?.Invoke();
        LoadInterstitialAd();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message){  }
    public void OnUnityAdsShowStart(string placementId){  }
}
