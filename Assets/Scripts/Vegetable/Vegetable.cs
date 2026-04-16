using System;
using UnityEngine;
using System.Collections.Generic;

public class Vegetable : MonoBehaviour
{
    public enum VegetableType {Default, Ice, Giant, Magic, Radiation, Reaper, Mutant, Warning, Virus, Enchanted}

    [Header("Состояние")]
    public VegetableType specialType = VegetableType.Default;
    public bool isActionReady;
    public bool IsImmune { get; set; }
    
    [Header("Настройки")]
    [HideInInspector] public Color currentTargetColor;
    public float radiusOffset = 0f; 
    // Статика для событий и счета
    //private static int numberOfDrops = 0;
    //public static int GetTotalDrops() => numberOfDrops;
    public static Action OnVegetableDropped;
    // Ссылки на компоненты и логику
    public object currentTimer;
    private Rigidbody2D rb;
    private VegetableVisual visual;
    private Hazard hazard;
    private Drag drag;
    // Кэш для сброса к дефолту
    private float originalMass; 
    private SpriteRenderer[] renderers;
    private Color[] originalColors;
    [Header("Масштабирование")]
    public float currentBaseScale = 1.35f; 

    
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
        isActionReady = true;
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
        //numberOfDrops++;
        OnVegetableDropped?.Invoke(); 

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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActionReady)
        {
            isActionReady = false; 
            if (TryGetEffectData(out var data) && data.effectLogic != null)
            { 
                data.effectLogic.OnImpact(this, collision);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayEffectSound(CurrentEffectData.effectSound);
               

            }
        }
    }

    public void ApplyEffectSettings(VegetableEffectData data)
    {
        if (data == null || rb == null) return;
        float multiplier = data.scaleMultiplier; 
        transform.localScale = Vector3.one * (currentBaseScale * multiplier);
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
        currentBaseScale = transform.localScale.x;
        if (type == VegetableType.Default)
        {
            SoftReset();
            return; 
        }
        if (visual != null) visual.UpdateVisuals(specialType);
        if (TryGetEffectData(out var data) && data.effectLogic != null)
        {
            data.effectLogic.OnStatusApplied(this);
        }
    }

    // --- СЛУЖЕБНЫЕ МЕТОДЫ ---
    public void SoftReset()
    {
        IsImmune = false;
        specialType = VegetableType.Default;
        currentTimer = null;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.mass = originalMass;
        }

        transform.localScale = Vector3.one * currentBaseScale;

        if (visual != null) visual.UpdateVisuals(specialType);
        if (hazard != null) hazard.ResetHazard();
    }


    public void HardResetForPool()
    {
        specialType = VegetableType.Default;
        currentBaseScale = 1.35f; 
        transform.localScale = Vector3.one * currentBaseScale;

        IsImmune = false;
        isActionReady = false;
        currentTimer = null;

        if (rb != null)
        {
            rb.simulated = false; // Выключаем в пуле
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = originalMass;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 1.85f;
            rb.drag = 0f;
        }
        if (visual != null) visual.UpdateVisuals(VegetableType.Default);
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

    public VegetableEffectData CurrentEffectData 
    {
        get 
        {
            TryGetEffectData(out var data);
            return data;
        }
    }
    public void LoadState(VegetableType type)
    {
        
        rb.simulated = true;
        
       // transform.SetParent(null);
       
        SetSpecialType(type);
       
        isActionReady = false;
    }


    // Методы-прослойки для зоны геймовера (чтобы не менять другие скрипты)
    public void UpdateHazardVisuals(float p) => hazard.UpdateHazard(p);
    public void OnZoneEnter() => hazard.OnZoneEnter();
    public void OnZoneExit() => hazard.OnZoneExit();
    public bool IsInHazardZone() => hazard.IsInHazardZone();
    public float hazardTimer { get => hazard.hazardTimer; set => hazard.hazardTimer = value; }
}
