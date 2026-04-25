using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string androidUnitID = "Rewarded_Android";
    public static event Action OnRewardedAdClosed;
    private Action onRewardGranted;
    public void LoadRewardedAd()
    {
        Advertisement.Load(androidUnitID, this);
    }
    public void ShowRewardedAd(Action rewardCallback)
    {
        onRewardGranted=rewardCallback;
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
        if (placementId == androidUnitID)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Reward granted");

                onRewardGranted?.Invoke();
                onRewardGranted = null; 
            }

            OnRewardedAdClosed?.Invoke();

          
            LoadRewardedAd();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }
    public void OnUnityAdsShowStart(string placementId) { }
}

