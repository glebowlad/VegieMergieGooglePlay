using System.Collections.Generic;
using UnityEngine;

public class VegetableVisual : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    public SpriteRenderer specialMask;
    public ParticleSystem auraParticles;

    public List<VegetableEffectData> allEffects;

    private SpriteRenderer[] mainRenderers;
    private Color[] originalColors;

    public void Init(SpriteRenderer[] renderers, Color[] colors)
    {
        mainRenderers = renderers;
        originalColors = colors;
        
        if (specialMask != null) specialMask.gameObject.SetActive(false);
        if (auraParticles != null) auraParticles.gameObject.SetActive(false);
    }

    public void UpdateVisuals(Vegetable.VegetableType type)
    {
        if (auraParticles != null)
            ResetParticlesToDefault();

        if (mainRenderers != null)
        {
            foreach (var r in mainRenderers)
            {
                if (r == null) continue;
                var c = r.color;
                c.a = 1f;
                r.color = c;
            }
        }

        if (type == Vegetable.VegetableType.Default)
        {
            if (specialMask != null) specialMask.gameObject.SetActive(false);
            if (auraParticles != null) auraParticles.gameObject.SetActive(false);

            if (mainRenderers != null)
            {
                foreach (var r in mainRenderers)
                {
                    if (r != null && r != specialMask) r.enabled = true;
                }
            }
            return;
        }

        VegetableEffectData data = null;
        if (allEffects != null)
        {
            for (int i = 0; i < allEffects.Count; i++)
            {
                var e = allEffects[i];
                if (e != null && e.type == type)
                {
                    data = e;
                    break;
                }
            }
        }

        if (data == null)
        {
            if (specialMask != null) specialMask.gameObject.SetActive(false);
            if (auraParticles != null) auraParticles.gameObject.SetActive(false);

            if (mainRenderers != null)
            {
                foreach (var r in mainRenderers)
                {
                    if (r != null && r != specialMask) r.enabled = true;
                }
            }
            return;
        }

        if (specialMask != null)
        {
            specialMask.gameObject.SetActive(true);
            specialMask.enabled = true;
            specialMask.color = data.maskColor;
            if (mainRenderers != null)
            {
                foreach (var r in mainRenderers)
                {
                    if (r != null && r != specialMask) r.enabled = false;
                }
            }
        }

        if (auraParticles != null)
        {
            auraParticles.gameObject.SetActive(true);

            var main = auraParticles.main;
            var emission = auraParticles.emission;
            var rotation = auraParticles.rotationOverLifetime;
            var textureSheet = auraParticles.textureSheetAnimation;
            var shape = auraParticles.shape;

            main.startColor = Color.white;
            shape.enabled = !data.disableShape;

            if (data.icon != null)
                textureSheet.SetSprite(0, data.icon);

            main.maxParticles = data.maxParticles;
            main.startLifetime = data.lifetime;
            emission.rateOverTime = data.emissionRate;
            main.startSize = data.size;
            main.startSpeed = data.speed;

            rotation.enabled = data.rotationEnabled;
            if (data.rotationEnabled)
                rotation.z = new ParticleSystem.MinMaxCurve(data.rotationSpeed.x, data.rotationSpeed.y);

            if (!auraParticles.isPlaying) auraParticles.Play();
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

    private void ResetParticlesToDefault()
    {
        if (auraParticles == null) return;

        var main = auraParticles.main;
        var emission = auraParticles.emission;
        var rotation = auraParticles.rotationOverLifetime;
        var shape = auraParticles.shape;

        main.startColor = Color.white;
        main.startSize = 30f;
        main.startSpeed = 0.1f;
        main.maxParticles = 10;
        main.startLifetime = 6.0f;
        emission.rateOverTime = 0.5f;
        rotation.enabled = false;
        shape.enabled = true;
    }


}
