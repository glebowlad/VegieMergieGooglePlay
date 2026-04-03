using UnityEngine;
using System.Collections.Generic;

public class Vegetable : MonoBehaviour
{
    public enum VegetableType 
    { 
        Default, 
        Ice, 
        Giant, 
        Magic,     
        Radiation, 
        Reaper,   
        Mutant,
        // Вторичные эффекты (не участвуют в рандоме спавна)
        Warning,   // Внимание Радиация
        Virus,     // Вирус (активная фаза Мутанта)
        Enchanted  // Эффект магии на цели   
    }
    
    [Header("Настройки")]
    public VegetableType specialType = VegetableType.Default;
    public float radiusOffset = 0f;
    
    // Скрываем из инспектора, чтобы не мешался, он считается в коде
    [HideInInspector] public Color currentTargetColor;

    [HideInInspector] public bool hasRecoveredFromToxic = false;

    private Rigidbody2D rb;
    private VegetableVisual visual;
    private Hazard hazard;
    private Drag drag;
    
    private SpriteRenderer[] renderers;
    private Color[] originalColors;
    

    private static int numberOfDrops = 0;
    public static int GetTotalDrops()
    {
        return numberOfDrops;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        visual = GetComponent<VegetableVisual>();
        // Добавляем Hazard программно, чтобы не вешать вручную
        hazard = gameObject.AddComponent<Hazard>(); 
        
        rb.simulated = false;

        // Собираем все спрайты, кроме маски
        SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>();
        List<SpriteRenderer> tempMain = new List<SpriteRenderer>();
        foreach (var r in allRenderers) 
        {
            if (visual != null && r != visual.specialMask) tempMain.Add(r);
        }
        
        renderers = tempMain.ToArray();
        originalColors = new Color[renderers.Length];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].color;
        }

        // Устанавливаем базовый цвет
        if (originalColors.Length > 0) currentTargetColor = originalColors[0];

        // Инициализируем компоненты
        if (visual != null) visual.Init(renderers, originalColors);
        if (hazard != null) hazard.Init(this, visual);
    }

    public void Initialize(Drag _drag)
    {
        drag = _drag;
        if (drag != null) 
        { 
            drag.WhileDrag += Move; 
            drag.OnDragFinished += Drop; 
        }
    }

    private void Move() => transform.position = transform.parent.position;

    private void Drop()
    {
        // --- РЕКЛАМА --------------
        numberOfDrops++;
        if (numberOfDrops != 0 && numberOfDrops % 70 == 0)
        {
            AdsManager.Instance.interstitialAds.ShowInterstitialAd();
        }
        // -------------------------

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        rb.simulated = true;

        if (visual != null) 
            visual.UpdateVisuals(specialType, currentTargetColor);
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDropSound();
        
        if (drag != null) 
        { 
            drag.WhileDrag -= Move; 
            drag.OnDragFinished -= Drop; 
        }
    }

    public void SetSpecialType(VegetableType type)
    {
        specialType = type;
        transform.localScale = Vector3.one * 1.35f;
        GetComponent<Collider2D>().isTrigger = false;

        switch (specialType)
        {
            case VegetableType.Giant: 
                currentTargetColor = new Color(0.8f, 0.3f, 0f); // Темная медь
                transform.localScale = Vector3.one * (1.35f * 1.5f); 
                if (gameObject.GetComponent<Giant>() == null) gameObject.AddComponent<Giant>(); 
                break;

            case VegetableType.Ice: 
                currentTargetColor = new Color(0f, 0.8f, 1f); // Ледяной голубой
                if (gameObject.GetComponent<Ice>() == null) gameObject.AddComponent<Ice>(); 
                break;

            case VegetableType.Magic: 
                // Эфирный розовато-лиловый цвет для магии уменьшения
                currentTargetColor = new Color(1f, 0.6f, 0.9f, 0.8f); 
                // Сюда позже добавим скрипт Magic (Mini-эффект по области)
                break;

            case VegetableType.Radiation: // Бывший Toxic
                currentTargetColor = new Color(0.6f, 0.9f, 0.2f); // Кислотно-желтый
                break;

            case VegetableType.Reaper: // Наш новый Жнец
                currentTargetColor = new Color(0.5f, 0f, 0.8f); // Глубокий фиолетовый
                
                // --- ТЕСТОВАЯ ГРАВИТАЦИЯ ДЛЯ ВИЗУАЛА ---
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null) 
                {
                    rb.gravityScale = 0.5f; // В 10 раз медленнее обычного
                    rb.drag = 2f;         // Сопротивление, чтобы "парил" как перышко
                }
                // ---------------------------------------

                break;

            case VegetableType.Mutant: // Инкубация (ДНК)
                currentTargetColor = new Color(0.6f, 0f, 0.1f, 1f); // Бордово-красный
                break;

            case VegetableType.Warning: 
                currentTargetColor = new Color(0.8f, 0.9f, 0.1f, 0.7f); // Желто-зеленый (Тревога)
                break;

            case VegetableType.Virus: 
                currentTargetColor = new Color(0.85f, 0.2f, 0.1f, 0.8f); // Гнойно-красный
                break;

            case VegetableType.Enchanted: 
                currentTargetColor = new Color(1f, 0.4f, 1f, 0.6f); // Магический розово-фиолетовый
                break;

            default: 
                if (originalColors.Length > 0) currentTargetColor = originalColors[0]; 
                break;
        }
    
        if (visual != null) visual.UpdateVisuals(specialType, currentTargetColor);
    }


    public void ResetToDefault()
    {
        hasRecoveredFromToxic = false;
        specialType = VegetableType.Default;
        transform.localScale = Vector3.one * 1.35f;
        
        if (rb != null) 
        { 
            rb.simulated = false; 
            rb.velocity = Vector2.zero; 
            rb.angularVelocity = 0f; 
        }

        GetComponent<Collider2D>().isTrigger = false;
        
        if (hazard != null) hazard.ResetHazard();
        if (originalColors.Length > 0) currentTargetColor = originalColors[0];
        if (visual != null) visual.UpdateVisuals(specialType, currentTargetColor);
    }

    // Методы-прослойки для зоны геймовера (чтобы не менять другие скрипты)
    public void UpdateHazardVisuals(float p) => hazard.UpdateHazard(p);
    public void OnZoneEnter() => hazard.OnZoneEnter();
    public void OnZoneExit() => hazard.OnZoneExit();
    public bool IsInHazardZone() => hazard.IsInHazardZone();
    public float hazardTimer { get => hazard.hazardTimer; set => hazard.hazardTimer = value; }
}
