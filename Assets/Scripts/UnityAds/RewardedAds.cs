using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string androidUnitID = "Rewarded_Android";

    public void LoadRewardedAd()
    {
        Advertisement.Load(androidUnitID, this);
    }
    public void ShowRewardedAd()
    {
        Advertisement.Show(androidUnitID, this);
        LoadRewardedAd();
    }
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Rewarded Ad loaded");
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) { }

    public void OnUnityAdsShowClick(string placementId) { }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) 
    { 
        if(placementId==androidUnitID && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Reward");
        } 
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }
    public void OnUnityAdsShowStart(string placementId) { }
}

