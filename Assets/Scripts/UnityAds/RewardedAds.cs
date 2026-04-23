using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string androidUnitID = "Rewarded_Android";
    public static event Action OnRewardedAdClosed;
    public void LoadRewardedAd()
    {
        Advertisement.Load(androidUnitID, this);
    }
    public void ShowRewardedAd()
    {
        Advertisement.Show(androidUnitID, this);
       
    }
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Rewarded Ad loaded");
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) { }

    public void OnUnityAdsShowClick(string placementId) { }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) 
    {
        Time.timeScale = 1f;

        // Если реклама досмотрена (награда получена) или пропущена
        OnRewardedAdClosed?.Invoke();
        LoadRewardedAd();
        if (placementId==androidUnitID && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Reward");
        } 
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }
    public void OnUnityAdsShowStart(string placementId) { }
}

