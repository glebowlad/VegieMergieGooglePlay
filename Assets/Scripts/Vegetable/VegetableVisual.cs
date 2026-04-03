using UnityEngine;

public class VegetableVisual : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    public SpriteRenderer specialMask;
    public ParticleSystem auraParticles;

    [Header("Основные иконки эффектов")]
    public Sprite iconIce;      
    public Sprite iconGiant;    
    public Sprite iconMagic;    
    public Sprite iconRadiation;
    public Sprite iconReaper;   
    public Sprite iconMutant;    // Бывший Virus, теперь это иконка ДНК

    [Header("Иконки статусов (для жертв)")]
    public Sprite statusMagic;    
    public Sprite statusWarning;  // Для иконки "Внимание"
    public Sprite statusVirus;    // Для активной фазы вируса

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
        // 1. СБРОС К ОБЫЧНОМУ ВИДУ (Твой рабочий код)
        if (type == Vegetable.VegetableType.Default)
        {
            if (specialMask != null) specialMask.gameObject.SetActive(false);
            if (auraParticles != null) auraParticles.gameObject.SetActive(false);
            
            foreach (var r in mainRenderers) {
                if (r != specialMask) r.enabled = true;
            }
            return;
        }

        // 2. ВКЛЮЧЕНИЕ МАСКИ (Твой рабочий код)
        if (specialMask != null)
        {
            specialMask.gameObject.SetActive(true);
            specialMask.enabled = true;
            specialMask.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
            foreach (var r in mainRenderers) {
                if (r != specialMask) r.enabled = false;
            }
        }

        // 3. НАСТРОЙКА ЧАСТИЦ
        if (auraParticles != null)
        {
            auraParticles.gameObject.SetActive(true);
            
            var main = auraParticles.main;
            var emission = auraParticles.emission;
            var rotation = auraParticles.rotationOverLifetime;
            var textureSheet = auraParticles.textureSheetAnimation;

            // Возвращаем дефолты, чтобы настройки от  не липли
            main.startColor = Color.white; 
            main.startSize = 30f;
            main.startSpeed = 0.1f;
            main.maxParticles = 10;
            main.startLifetime = 6.0f;
            emission.rateOverTime = 0.5f;
            rotation.enabled = false;
            var resetShape = auraParticles.shape;
            resetShape.enabled = true; // Возвращаем родную форму префаба



            switch (type)
            {
                case Vegetable.VegetableType.Ice: 
                    textureSheet.SetSprite(0, iconIce);
                    main.maxParticles = 5; 
                    main.startLifetime = 2.2f; 
                    emission.rateOverTime = 6.5f; 
                    main.startSize = 40f; 
                    rotation.enabled = true;
                    rotation.z = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);
                    main.startSpeed = 0.1f; 
                    break;

                case Vegetable.VegetableType.Giant:
                    textureSheet.SetSprite(0, iconGiant);
                    var giantShape = auraParticles.shape;
                    giantShape.enabled = false; 
                    main.maxParticles = 1;
                    main.startLifetime = 5.0f;
                    emission.rateOverTime = 5.0f; 
                    main.startSize = 65f;
                    main.startColor = new Color(1f, 1f, 1f, 0.9f);
                    break;


                case Vegetable.VegetableType.Magic: 
                    textureSheet.SetSprite(0, iconMagic);
                    main.maxParticles = 3;           
                    main.startSize = 75f;            
                    main.startLifetime = 7.0f;       
                    emission.rateOverTime = 3.0f;    
                    main.startSpeed = 0.15f;         
                    main.startColor = new Color(1f, 1f, 1f, 0.9f);
                    rotation.enabled = true;
                    rotation.z = new ParticleSystem.MinMaxCurve(-0.4f, 0.4f); // Покачивание
                    break;

                case Vegetable.VegetableType.Reaper: 
                    textureSheet.SetSprite(0, iconReaper);
                    main.maxParticles = 4; 
                    emission.rateOverTime = 3.0f; 
                    main.startLifetime = 1.5f;
                    main.startSpeed = 0.5f; 
                    main.startSize = 70f; 
                    rotation.enabled = true;
                    rotation.z = new ParticleSystem.MinMaxCurve(1.5f, 3.0f); 
                    break;

                case Vegetable.VegetableType.Radiation: 
                    textureSheet.SetSprite(0, iconRadiation);
                    main.maxParticles = 3;           
                    emission.rateOverTime = 1.66f;   
                    main.startLifetime = 1.8f;      
                    main.startSize = 65f;           
                    main.startSpeed = 0.2f;    
                    rotation.enabled = false; 
                    break;

                case Vegetable.VegetableType.Mutant:
                    textureSheet.SetSprite(0, iconMutant); 
                    main.maxParticles = 3;
                    main.startLifetime = 3.0f;
                    emission.rateOverTime = 0.5f;
                    main.startSize = 65f;
                    main.startSpeed = 0.12f;
                    rotation.enabled = false;
                    break;

                case Vegetable.VegetableType.Warning:
                    textureSheet.SetSprite(0, statusWarning);
                    main.maxParticles = 1;
                    main.startLifetime = 2.0f;
                    emission.rateOverTime = 20.0f; 
                    main.startSize = 75f;
                    main.startSpeed = 0f;
                    rotation.enabled = false;
                    break;

                case Vegetable.VegetableType.Virus:
                    textureSheet.SetSprite(0, statusVirus);
                    main.maxParticles = 6;           // Больше частиц (споры)
                    main.startLifetime = 1.2f;       // Короткая жизнь
                    emission.rateOverTime = 5.0f;    // Плотный поток
                    
                    main.startSize = 65f;            // Чуть мельче
                    main.startSpeed = 0.9f;          // Быстрее разлетаются
                    rotation.enabled = true;
                    rotation.z = new ParticleSystem.MinMaxCurve(-0.4f, 0.4f); // Хаотичное вращение
                    break;


                case Vegetable.VegetableType.Enchanted:
                    textureSheet.SetSprite(0, statusMagic);
                    main.maxParticles = 4;
                    main.startLifetime = 3.5f;
                    emission.rateOverTime = 1.15f; 
                    main.startSize = 50f;
                    main.startSpeed = 0.05f;
                    rotation.enabled = true;
                    rotation.z = new ParticleSystem.MinMaxCurve(-1.0f, 1.0f);
                    break;

            }

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
        var main = auraParticles.main;
        var emission = auraParticles.emission;
        var rotation = auraParticles.rotationOverLifetime;

        // Устанавливаем "заводские" настройки (для обычного овоща)
        main.startLifetime = 6.0f;
        main.startSize = 40f; 
        main.maxParticles = 3;
        emission.rateOverTime = 0.5f;
        rotation.enabled = false;
        main.startSpeed = 0.1f;
    }


}
