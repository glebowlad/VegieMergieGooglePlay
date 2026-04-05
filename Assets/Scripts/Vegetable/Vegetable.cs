using System;
using UnityEngine;
using System.Collections.Generic;

public class Vegetable : MonoBehaviour
{
    public enum VegetableType {Default, Ice, Giant, Magic, Radiation, Reaper, Mutant, Warning, Virus, Enchanted}

    [Header("Настройки")]
    public VegetableType specialType = VegetableType.Default;
    
    public int currentTurns;
    public bool IsImmune { get; set; }

    [HideInInspector] public Color currentTargetColor;
    public float radiusOffset = 0f; 

    private static int numberOfDrops = 0;
    public static int GetTotalDrops() => numberOfDrops;

    public object currentTimer;
    private Rigidbody2D rb;
    private VegetableVisual visual;
    private Hazard hazard;
    private Drag drag;
    private float originalMass; 
    
    private SpriteRenderer[] renderers;
    private Color[] originalColors;

    public static Action OnVegetableDropped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) originalMass = rb.mass; 

        visual = GetComponent<VegetableVisual>();
        hazard = gameObject.AddComponent<Hazard>(); 
        rb.simulated = false;

        // Сбор спрайтов для масок (твоя логика)
        SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>();
        List<SpriteRenderer> tempMain = new List<SpriteRenderer>();
        foreach (var r in allRenderers) 
        {
            if (visual != null && r != visual.specialMask) tempMain.Add(r);
        }
        renderers = tempMain.ToArray();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) originalColors[i] = renderers[i].color;

        if (visual != null) visual.Init(renderers, originalColors);
        if (hazard != null) hazard.Init(this, visual);
    }

    public void Initialize(Drag _drag)
    {
        drag = _drag;
    }

    public void EnableInput()
    {
        if (drag == null) return;
        
        // Принудительно отписываемся от старого (защита от дублей)
        drag.WhileDrag -= Move;
        drag.OnDragFinished -= Drop;

        // Подписываемся заново
        drag.WhileDrag += Move;
        drag.OnDragFinished += Drop;
    }

    private void Move() => transform.position = transform.parent.position;

    private void Drop()
    {
        // Логика рекламы (каждые 70 бросков)
        numberOfDrops++;
        OnVegetableDropped?.Invoke(); 
        if (numberOfDrops != 0 && numberOfDrops % 70 == 0)
        {
            if (AdsManager.Instance != null && AdsManager.Instance.interstitialAds != null)
                AdsManager.Instance.interstitialAds.ShowInterstitialAd();
        }

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        rb.simulated = true;

        // Обновляем визуал и физику при броске
        if (visual != null) visual.UpdateVisuals(specialType);
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDropSound();
        
        // Отписываемся от событий, чтобы не двигать упавший овощ
        if (drag != null) 
        { 
            drag.WhileDrag -= Move; 
            drag.OnDragFinished -= Drop; 
        }
    }
    //изменить,добавить флаг
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (TryGetEffectData(out var data) && data.effectLogic != null)
        {
            data.effectLogic.OnImpact(this, collision);
        }
    }

    public void ApplyEffectSettings(VegetableEffectData data)
    {
        if (data == null || rb == null) return;
        transform.localScale = Vector3.one * (1.35f * data.scaleMultiplier);
        rb.useAutoMass = false;
        rb.mass = (data.fixedMass > 0) ? data.fixedMass : originalMass;
        
        rb.gravityScale = data.gravityScale;
        rb.drag = data.linearDrag;
        rb.angularDrag = 0.2f;
    }

    public void SetSpecialType(VegetableType type)
    {
        specialType = type;
        // Сбрасываем триггер, если он был включен ранее
        GetComponent<Collider2D>().isTrigger = false;
        if (type == VegetableType.Default)
        {
            ResetToDefault(); 
            return; 
        }
        // Вся настройка теперь идет через визуальный скрипт и ассеты
        if (visual != null) visual.UpdateVisuals(specialType);
        
        RefreshTurnsFromEffectData();
    }

    //на удаление
    private void Update()
    {
        if (specialType == VegetableType.Default) return;
        if (!TryGetEffectData(out var data) || data == null) return;

        // ТРЯСКА (для Радиации/Вируса)
        if (data.hasShake && visual != null && visual.specialMask != null)
        {
            float offset = data.shakeIntensity;
            visual.specialMask.transform.localPosition = new Vector3(
                UnityEngine.Random.Range(-offset, offset),
                UnityEngine.Random.Range(-offset, offset), 0
            );
        }
    }
    //на удаление
    public void TickTurn()
    {
        if (specialType == VegetableType.Default || IsImmune) return;
        if (!TryGetEffectData(out var data) || data == null) return;
        if (currentTurns > 0)
        {
            currentTurns--;
            if (currentTurns <= 0)
            {
                if (data.turnsToEvolve > 0)
                {
                    SetSpecialType(data.evolutionType);
                }
                else
                {
                    ResetToDefault();
                }
            }
        }

        // 4. СПЕЦ-ЛОГИКА ДЛЯ WARNING (Рост и Мутация)
        if (specialType == VegetableType.Warning)
        {
            transform.localScale *= 1.05f;
            if (UnityEngine.Random.value < 0.01f) SetSpecialType(VegetableType.Mutant);
        }
    }


    // --- СЛУЖЕБНЫЕ МЕТОДЫ ---
    public void ResetToDefault()
    {
        IsImmune = false;
        currentTurns = 0;
        specialType = VegetableType.Default;
        
        if (rb != null) 
        { 
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = originalMass; 
            rb.gravityScale = 1f;
            rb.drag = 0f;
            rb.velocity = Vector2.zero; 
            rb.angularVelocity = 0f; 
        }

        transform.localScale = Vector3.one * 1.35f;
        if (visual != null) visual.UpdateVisuals(specialType);
        if (hazard != null) hazard.ResetHazard();
    }

    private bool TryGetEffectData(out VegetableEffectData data)
    {
        data = null;
        if (visual == null || visual.allEffects == null) return false;
        foreach (var e in visual.allEffects)
        {
            if (e != null && e.type == specialType) { data = e; return true; }
        }
        return false;
    }
    //на удаление,будильник
    private void RefreshTurnsFromEffectData()
    {
        currentTurns = 0;
        if (specialType == VegetableType.Default) return;
        if (TryGetEffectData(out var data))
        {
            currentTurns = (data.turnsToEvolve > 0) ? data.turnsToEvolve : data.turnsToLive;
        }
    }

    public VegetableEffectData CurrentEffectData 
    {
        get 
        {
            TryGetEffectData(out var data);
            return data;
        }
    }

    // Методы-прослойки для зоны геймовера (чтобы не менять другие скрипты)
    public void UpdateHazardVisuals(float p) => hazard.UpdateHazard(p);
    public void OnZoneEnter() => hazard.OnZoneEnter();
    public void OnZoneExit() => hazard.OnZoneExit();
    public bool IsInHazardZone() => hazard.IsInHazardZone();
    public float hazardTimer { get => hazard.hazardTimer; set => hazard.hazardTimer = value; }
}
