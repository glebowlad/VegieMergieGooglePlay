using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAds : MonoBehaviour, IUnityAdsInitializationListener
{
    private string androidID = "6069177";
    public bool isTesting = true;

    public void OnInitializationComplete()
    {
        Debug.Log("Ads initialized");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message){ }

    void Awake()
    {
        if(!Advertisement.isInitialized&& Advertisement.isSupported)
        {
            Advertisement.Initialize(androidID, isTesting, this);
        }
    }

  
}
