using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAds : MonoBehaviour, IUnityAdsInitializationListener
{
    private string androidID = "6069177";
    public bool isTesting = false;
    public static event Action OnAdsInitialized;
    public void OnInitializationComplete()
    {
        Debug.Log("Ads initialized");
        OnAdsInitialized?.Invoke();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message){ }

    void Awake()
    {
        if(!Advertisement.isInitialized&& Advertisement.isSupported)
        {
            Advertisement.Initialize(androidID, isTesting, this);
        }
        else if (Advertisement.isInitialized)
        {
            // ≈сли вдруг SDK уже был инициализирован ранее
            OnAdsInitialized?.Invoke();
        }
    }

  
}
