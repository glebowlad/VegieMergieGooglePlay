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
        if (auraParticles != null) ResetParticlesToDefault();

        if (mainRenderers != null)
        {
            foreach (var r in mainRenderers)
            {
                if (r == null) continue;
                Color c = r.color;
                c.a = 1f;
                r.color = c;
            }
        }

        var veg = GetComponent<Vegetable>();
        if (type == Vegetable.VegetableType.Default)
        {
            if (specialMask != null) specialMask.gameObject.SetActive(false);
            if (auraParticles != null) auraParticles.gameObject.SetActive(false);
            if (mainRenderers != null)
            {
                foreach (var r in mainRenderers)
                    if (r != null && r != specialMask) r.enabled = true;
            }
            transform.localScale = Vector3.one * 1.35f;
            return;
        }

        var data = allEffects?.Find(e => e != null && e.type == type);
        if (data != null)
        {
            veg.ApplyEffectSettings(data);

            if (specialMask != null)
            {
                specialMask.gameObject.SetActive(true);
                specialMask.enabled = true;
                specialMask.color = data.maskColor;
                //specialMask.sprite = data.icon;
                foreach (var r in mainRenderers)
                    if (r != null && r != specialMask) r.enabled = true;
            }

            if (auraParticles != null)
            {
                auraParticles.gameObject.SetActive(true);
                auraParticles.Stop();
                auraParticles.Clear();

                var ts = auraParticles.textureSheetAnimation;
                ts.SetSprite(0, data.icon);

                var main = auraParticles.main;
                main.maxParticles = data.maxParticles;
                main.startLifetime = data.lifetime;
                main.startSize = data.size;
                main.startSpeed = data.speed;

                var em = auraParticles.emission;
                em.rateOverTime = data.emissionRate;

                var rot = auraParticles.rotationOverLifetime;
                rot.enabled = data.rotationEnabled;
                if (data.rotationEnabled)
                    rot.z = new ParticleSystem.MinMaxCurve(data.rotationSpeed.x, data.rotationSpeed.y);

                var shape = auraParticles.shape;
                shape.enabled = !data.disableShape;

                auraParticles.Play();
            }
        }
    }



    public void SetHazardColor(Color hazardColor, bool isSpecial)
    {
        if (specialMask == null) return;
        specialMask.gameObject.SetActive(true);
        specialMask.enabled = true;

        if (isSpecial)
        {
            var v = GetComponent<Vegetable>();
            var data = allEffects?.Find(e => e != null && v != null && e.type == v.specialType);
            if (data != null)
            {
                Color blended = Color.Lerp(data.maskColor, Color.red, hazardColor.a * 0.6f);
                blended.a = data.maskColor.a;
                specialMask.color = blended;
            }
        }
        else
        {
            Color c = Color.red;
            c.a = hazardColor.a * 0.5f;
            specialMask.color = c;
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
