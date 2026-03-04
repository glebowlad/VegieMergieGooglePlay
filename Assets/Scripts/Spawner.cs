using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] vegPrefabs;
    [SerializeField]
    private GameObject gameOverLine;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private Image nextItemImage;

    private GameObject itemToSpawn;
    private GameObject nextItemToSpawn;
    private RectTransform spawnerRect;
    private PrefabPool pool;
    private float itemWidth;
    private Drag drag;
    public static bool IsSpawned { get; private set; }
    public float CurrentItemWidth => itemWidth; 


    private void Awake()
    {
        spawnerRect = gameObject.GetComponent<RectTransform>();
        gameOverPanel.SetActive(false);
        nextItemImage.enabled = false;
        pool = new PrefabPool(vegPrefabs,10);
        drag = GetComponent<Drag>();
        Subscribe(drag);
        Spawn();
    }

    private void Spawn()
    {
        IsSpawned = false;
        StopAllCoroutines();
        StartCoroutine(SpawnTimer());

    }
    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(0.4f);

        if (nextItemToSpawn == null)
        {
            itemToSpawn = pool.GetRandom();
        }
        else
        {
            itemToSpawn = nextItemToSpawn;
            itemToSpawn.SetActive(true);
        }

        nextItemToSpawn = pool.GetRandom();
        nextItemToSpawn.SetActive(false);

        nextItemImage.sprite = nextItemToSpawn.GetComponent<SpriteRenderer>().sprite;
        nextItemImage.enabled = true;




        IsSpawned = true;
        itemToSpawn.transform.SetParent(transform, false);

        // Получаем скрипт овоща для инициализации и доступа к его настройкам
        var itemVeg = itemToSpawn.GetComponent<Vegetable>();
        itemVeg.Initialize(drag, gameOverLine, gameOverPanel);

        // Рассчитываем ширину с учетом индивидуального отступа овоща
        // Если овощ "цепляет" стенку — в инспекторе префаба ставим radiusOffset больше 0
        // Если "не доходит" — ставим меньше 0
        itemWidth = itemToSpawn.GetComponent<RectTransform>().rect.width + itemVeg.radiusOffset;

        // Обновляем размер спавнера (это нужно для корректной работы Drag)
        spawnerRect.sizeDelta = new Vector2(itemWidth, spawnerRect.sizeDelta.y);
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
