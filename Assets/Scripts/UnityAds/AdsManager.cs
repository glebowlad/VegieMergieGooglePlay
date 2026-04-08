using UnityEngine;
using System.Collections;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    [Header("Настройки таймеров")]
    [SerializeField] private float timeBetweenAds = 180f;   
    [SerializeField] private float minRestartCooldown = 60f; 

    private InterstitialAds interstitialAds;
    private RewardedAds rewardedAds;

    public RewardedAds RewardedAds => rewardedAds;

    private float lastAdShowTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        lastAdShowTime = Time.realtimeSinceStartup;

        interstitialAds = GetComponent<InterstitialAds>();
        rewardedAds = GetComponent<RewardedAds>();
    }

    private void Start()
    {
        if (interstitialAds != null) interstitialAds.LoadInterstitialAd();
        if (rewardedAds != null) rewardedAds.LoadRewardedAd();

        StartCoroutine(AutoAdRoutine());
    }

    private IEnumerator AutoAdRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            float timePassed = Time.realtimeSinceStartup - lastAdShowTime;
            if (timePassed >= timeBetweenAds)
            {
                Debug.Log($"<color=yellow>[Ads]</color> Авто-показ! Прошло {timePassed:F1} сек.");
                ShowSmartInterstitial();
            }
        }
    }

    public void ShowAdOnRestart()
    {
        float timePassed = Time.realtimeSinceStartup - lastAdShowTime;
        
        if (timePassed >= minRestartCooldown)
        {
            Debug.Log($"<color=green>[Ads]</color> Рестарт одобрен. Прошло {timePassed:F1} сек.");
            ShowSmartInterstitial();
        }
        else
        {
            Debug.Log($"<color=red>[Ads]</color> Рестарт заблокирован. Прошло всего {timePassed:F1} сек.");
        }
    }

    private void ShowSmartInterstitial()
    {
        lastAdShowTime = Time.realtimeSinceStartup;
        if (UnityEngine.Advertisements.Advertisement.isInitialized && interstitialAds != null)
        {
            interstitialAds.ShowInterstitialAd();
        }
        else
        {
            Debug.LogWarning("<color=orange>[Ads]</color> Попытка показа провалена: SDK еще не инициализирован.");
        }
    }

}
