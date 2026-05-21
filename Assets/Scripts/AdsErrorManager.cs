using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsErrorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject networkErrorWindow; 
    [SerializeField]
    private Button closeErrorWindowButton;
    [SerializeField]
    private InitializeAds initializeAds;
    [SerializeField]
    private Drag drag;
    void Start()
    {
        if (networkErrorWindow != null)
        {
            networkErrorWindow.SetActive(false);
        }
        if (closeErrorWindowButton != null)
        {
            closeErrorWindowButton.onClick.AddListener(HideNetworkErrorWindow);
        }
    }
    public void ShowNetworkErrorWindow()
    {
        if (networkErrorWindow != null)
        {
            networkErrorWindow.SetActive(true);
            Time.timeScale = 0f;
            drag.enabled = false;
        }
    }
    public void HideNetworkErrorWindow()
    {
        if (networkErrorWindow != null)
        {
            initializeAds.InitializeAd();
            networkErrorWindow.SetActive(false);
            Time.timeScale = 1f;
            drag.enabled = true;
            if (AdsManager.Instance != null && AdsManager.Instance.RewardedAds != null)
            {
                AdsManager.Instance.RewardedAds.RetryLoadAd();
            }
        }
    }
    private void OnDestroy()
    {
        if (closeErrorWindowButton != null)
        {
            closeErrorWindowButton.onClick.RemoveListener(HideNetworkErrorWindow);
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            HideNetworkErrorWindow();
    }
}
