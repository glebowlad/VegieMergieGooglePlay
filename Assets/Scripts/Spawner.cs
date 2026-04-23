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
        spawnerRect = gameObject.GetComponent<RectTransform>();
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
        yield return new WaitForSecondsRealtime(0.35f);

        // 1. Получаем объект из пула или используем заготовленный
        if (nextItemToSpawn == null)
        {
            itemToSpawn = pool.GetRandom();
            itemToSpawn.GetComponent<Vegetable>().HardResetForPool();
        }
        else
        {
            itemToSpawn = nextItemToSpawn;
            itemToSpawn.SetActive(true);
        }

        // 2. Готовим СЛЕДУЮЩИЙ овощ (превью)
        nextItemToSpawn = pool.GetRandom();
        var nextVeg = nextItemToSpawn.GetComponent<Vegetable>();
        
        // Сначала полный сброс
        nextVeg.HardResetForPool();
        nextItemToSpawn.SetActive(false);

        // Шанс 30% на спецэффект
        if (UnityEngine.Random.value <= currentEffectChance) //0.30f
        {
            nextVeg.SetSpecialType(GetPseudoRandomEffectType());  //Ice 1, Giant 2, Magic 3,  Radiation 4, Reaper 5,  Mutant 6, // Warning 7, Virus 8, Enchanted 9
            currentEffectChance = baseEffectChance;
            
        }
        else
        {
            currentEffectChance += chanceStep;
        }

            // 3. Обновляем UI превью (БЕЗ ДУБЛИКАТОВ ПЕРЕМЕННЫХ)
            var sRenderer = nextItemToSpawn.GetComponent<SpriteRenderer>(); // Используем короткое имя, чтобы не путаться
        if (sRenderer != null)
        {
            nextItemImage.sprite = sRenderer.sprite;
            nextItemImage.color = sRenderer.color; // Подхватит цвет спецэффекта
            nextItemImage.enabled = true;
        }

        // 4. Настраиваем текущий овощ для броска
        itemToSpawn.transform.SetParent(transform, false);
        var itemVeg = itemToSpawn.GetComponent<Vegetable>();
        itemVeg.Initialize(drag);
        itemVeg.EnableInput(); 

        itemWidth = itemToSpawn.GetComponent<RectTransform>().rect.width + itemVeg.radiusOffset;
        spawnerRect.sizeDelta = new Vector2(itemWidth, spawnerRect.sizeDelta.y);
        
        IsSpawned = true;
        isSpawning = false;
    }

    public void ForceResetSpawning()
    {
        // Если корутина зависла из-за паузы рекламы, сбрасываем флаги
        isSpawning = false;
        if (!IsSpawned)
        {
            Spawn();
        }
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
