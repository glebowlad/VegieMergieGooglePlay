using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] vegPrefabs;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private Image nextItemImage;

    private GameObject itemToSpawn;
    private GameObject nextItemToSpawn;
    private RectTransform spawnerRect;
    private PrefabPool pool;
    private float itemWidth;
    private float baseEffectChance = 0; //0.02f;
    private float currentEffectChance = 0; //0.02f;
    private float chanceStep = 0.018f;
    private Drag drag;
    private bool isSpawning=false;
    public  bool IsSpawned { get; private set; }
    public float CurrentItemWidth => itemWidth; 

    public GameObject GetCurrentItem() => itemToSpawn; //нужно для дебаг панели выбора эффекта
    public GameObject[] GetPrefabsArray() => vegPrefabs;

    private int[] effectCounts = new int[7];

    private void Awake()
    {
        spawnerRect = GetComponent<RectTransform>();
        gameOverPanel.SetActive(false);
        nextItemImage.enabled = false;

    
        pool = new PrefabPool(vegPrefabs, 4);
        drag = GetComponent<Drag>();
        Subscribe(drag);
        Spawn();
    }
    private void OnEnable()
    {
        InterstitialAds.OnAdClosed += ForceResetSpawning;
        RewardedAds.OnRewardedAdClosed += ForceResetSpawning;
    }

    private void OnDisable()
    {
        InterstitialAds.OnAdClosed -= ForceResetSpawning;
        RewardedAds.OnRewardedAdClosed -= ForceResetSpawning;
    }
    private Vegetable.VegetableType GetPseudoRandomEffectType()
    {
        float totalWeight = 0f;
        float[] weights = new float[7];

        for (int i = 1; i <= 6; i++)
        {
            weights[i] = 1f / (1 + effectCounts[i]);
            totalWeight += weights[i];
        }

        float roll = UnityEngine.Random.value * totalWeight;
        float sum = 0f;

        for (int i = 1; i <= 6; i++)
        {
            sum += weights[i];
            if (roll <= sum)
            {
                effectCounts[i]++;
                return (Vegetable.VegetableType)i;
            }
        }

        effectCounts[1]++;
        return (Vegetable.VegetableType)1;
    }

    private void Spawn()
    {
        if (isSpawning) return;
        
        StartCoroutine(SpawnTimer());

    }

    private IEnumerator SpawnTimer()
    {
        isSpawning = true;
        IsSpawned = false;
        yield return new WaitForSeconds(0.35f);
        if (IsSpawned) yield break;
        // 1. Настройка текущего овоща (itemToSpawn)
        if (nextItemToSpawn == null)
        {
            itemToSpawn = pool.GetRandom();
        }
        else
        {
            itemToSpawn = nextItemToSpawn;
            itemToSpawn.SetActive(true);
        }

        // КЭШИРУЕМ компоненты текущего овоща
        var itemVeg = itemToSpawn.GetComponent<Vegetable>();
        var itemRect = itemToSpawn.GetComponent<RectTransform>();

        if (nextItemToSpawn == null) itemVeg.HardResetForPool();

        // 2. Готовим СЛЕДУЮЩИЙ овощ (nextItemToSpawn)
        nextItemToSpawn = pool.GetRandom();

        // КЭШИРУЕМ компоненты следующего овоща
        var nextVeg = nextItemToSpawn.GetComponent<Vegetable>();
        var nextRenderer = nextItemToSpawn.GetComponent<SpriteRenderer>();

        nextVeg.HardResetForPool();
        nextItemToSpawn.SetActive(false);

        // Логика спецэффекта
        if (UnityEngine.Random.value <= currentEffectChance)
        {
            nextVeg.SetSpecialType(GetPseudoRandomEffectType());
            currentEffectChance = baseEffectChance;
        }
        else
        {
            currentEffectChance += chanceStep;
        }

        // 3. Обновляем UI превью через кэшированный nextRenderer
        if (nextRenderer != null)
        {
            nextItemImage.sprite = nextRenderer.sprite;
            nextItemImage.color = nextRenderer.color;
            nextItemImage.enabled = true;
        }

        // 4. Настраиваем текущий овощ для броска через кэшированный itemVeg и itemRect
        itemToSpawn.transform.SetParent(transform, false);
        itemVeg.Initialize(drag);
        itemVeg.EnableInput();

        itemWidth = itemRect.rect.width + itemVeg.radiusOffset;
        spawnerRect.sizeDelta = new Vector2(itemWidth, spawnerRect.sizeDelta.y);

        IsSpawned = true;
        isSpawning = false;
    }
    public void ForceResetSpawning()
    {
        isSpawning = false;
    }
    private void Subscribe(Drag _drag)
    {
        _drag = drag;
        drag.OnDragFinished += Spawn;
    }
    private void OnDestroy()
    {
        drag.OnDragFinished -= Spawn;
    }
}
