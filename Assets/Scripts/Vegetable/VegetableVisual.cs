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
                var ts = auraParticles.textureSheetAnimation;
                bool isSameEffect = auraParticles.isPlaying && ts.GetSprite(0) == data.icon;

                if (!isSameEffect) 
                {
                    auraParticles.gameObject.SetActive(true);
                    auraParticles.Stop();
                    auraParticles.Clear();

                    //var ts = auraParticles.textureSheetAnimation;
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
    }



    public void SetHazardColor(Color hazardColor, bool isSpecial)
    {
        if (specialMask == null) return;
        specialMask.gameObject.SetActive(true);
        specialMask.enabled = true;
        float rapidProgress = Mathf.Clamp01(hazardColor.a / 0.6f); 

        if (isSpecial)
        {
            var v = GetComponent<Vegetable>();
            var data = allEffects?.Find(e => e != null && v != null && e.type == v.specialType);
            if (data != null)
            {
                Color blended = Color.Lerp(data.maskColor, Color.red, rapidProgress * 1f);
                blended.a = data.maskColor.a;
                specialMask.color = blended;
            }
        }
        else
        {
            Color c = Color.red;
            c.a = rapidProgress * 1f;
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
}
