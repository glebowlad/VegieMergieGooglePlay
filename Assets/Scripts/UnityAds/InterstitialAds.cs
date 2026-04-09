using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string androidUnitID = "Interstitial_Android";

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

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message){  }

    public void OnUnityAdsShowClick(string placementId){  }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    { 
        LoadInterstitialAd();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message){  }
    public void OnUnityAdsShowStart(string placementId){  }
}
