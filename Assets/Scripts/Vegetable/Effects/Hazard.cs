using UnityEngine;

public class Hazard : MonoBehaviour
{
    private Vegetable vegetable;
    private VegetableVisual visual;
    
    [HideInInspector] public float hazardTimer = 0f;
    private int zoneCollidersCount = 0;

    // Инициализация связей
    public void Init(Vegetable veg, VegetableVisual vis)
    {
        vegetable = veg;
        visual = vis;
    }

    // Логика поведения в зоне риска
    public void UpdateHazard(float progress)
    {
        // Если этот овощ — Жнец, выходим из метода и не считаем таймер
        if (vegetable.specialType == Vegetable.VegetableType.Reaper) return;
        
        // 1. Тряска (чем ближе к геймоверу, тем сильнее)
        if (progress > 0.4f)
        {
            float intensity = (progress - 0.3f) * 0.01f;
            transform.position += (Vector3)Random.insideUnitCircle * intensity;
        }

        // 2. Покраснение (просим визуальный скрипт сменить цвет)
        if (visual != null && vegetable != null)
        {
            Color targetColor = Color.red;
            targetColor.a = progress; // От 0.0 (начало) до 1.0 (геймовер)

            bool isSpecial = vegetable.specialType != Vegetable.VegetableType.Default;
            visual.SetHazardColor(targetColor, isSpecial);
        }
    }

    // Управление счетчиком входа в зону
    public void OnZoneEnter() => zoneCollidersCount++;
    
    public void OnZoneExit() 
    { 
        zoneCollidersCount--; 
        if (zoneCollidersCount <= 0) ResetHazard(); 
    }

    public bool IsInHazardZone() => zoneCollidersCount > 0;

    // Сброс состояния, когда овощ покинул зону или игра перезапустилась
    public void ResetHazard()
    {
        hazardTimer = 0f;
        zoneCollidersCount = 0;
        if (visual != null && vegetable != null) 
        {
            visual.UpdateVisuals(vegetable.specialType);
        }
    }
}
