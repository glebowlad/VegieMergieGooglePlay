using UnityEngine;

public class VegetableVisual : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    public SpriteRenderer specialMask;
    public ParticleSystem auraParticles;

    [Header("Иконки эффектов")]
    public Sprite iconIce;
    public Sprite iconToxic;
    public Sprite iconGiant;
    public Sprite iconRebirth;

    private SpriteRenderer[] mainRenderers;
    private Color[] originalColors;

    public void Init(SpriteRenderer[] renderers, Color[] colors)
    {
        mainRenderers = renderers;
        originalColors = colors;
        
        if (specialMask != null) specialMask.gameObject.SetActive(false);
        if (auraParticles != null) auraParticles.gameObject.SetActive(false);
    }

    public void UpdateVisuals(Vegetable.VegetableType type, Color targetColor)
    {
        // 1. Сброс к обычному виду
        if (type == Vegetable.VegetableType.Default)
        {
            if (specialMask != null) specialMask.gameObject.SetActive(false);
            if (auraParticles != null) auraParticles.gameObject.SetActive(false);
            
            foreach (var r in mainRenderers) {
                if (r != specialMask) r.enabled = true;
            }
            return;
        }

        // 2. Включение маски
        if (specialMask != null)
        {
            specialMask.gameObject.SetActive(true);
            specialMask.enabled = true;
            specialMask.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
            
            foreach (var r in mainRenderers) {
                if (r != specialMask) r.enabled = false;
            }
        }

        // 3. Настройка частиц
        if (auraParticles != null)
        {
            auraParticles.gameObject.SetActive(true);
            var textureSheet = auraParticles.textureSheetAnimation;
            var main = auraParticles.main;
            main.startColor = targetColor;

            switch (type)
            {
                case Vegetable.VegetableType.Ice: textureSheet.SetSprite(0, iconIce); break;
                case Vegetable.VegetableType.Toxic: textureSheet.SetSprite(0, iconToxic); break;
                case Vegetable.VegetableType.Giant: textureSheet.SetSprite(0, iconGiant); break;
                case Vegetable.VegetableType.Rebirth: textureSheet.SetSprite(0, iconRebirth); break;
            }
            auraParticles.Play();
        }
    }

    public void SetHazardColor(Color color, bool isSpecial)
    {
        // Если овощ специальный — краснеет его маска
        if (isSpecial && specialMask != null)
        {
            specialMask.color = color;
        }
        else // Если обычный — краснеют основные спрайты
        {
            if (mainRenderers != null)
            {
                foreach (var r in mainRenderers) if (r != null) r.color = color;
            }
        }
    }

    public void ResetVisualsToNormal()
    {
        if (specialMask != null) specialMask.gameObject.SetActive(false);
        if (mainRenderers != null)
        {
            foreach (var r in mainRenderers) if (r != null) r.enabled = true;
        }
    }

}
