using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectData", menuName = "Vegetable/Effect Data")]
public class VegetableEffectData : ScriptableObject
{
    public Vegetable.VegetableType type;
    public Sprite icon;
    public Color maskColor = Color.white;

    public EffectAction effectLogic;

    [Header("Physics & Size")]
    public float scaleMultiplier = 1f;
    public float gravityScale = 1f;
    public float linearDrag = 0f;
    public float fixedMass = 0f; // 0 — оставить родной вес, >0 — принудительный вес (180 для Гиганта)


    [Header("Timing & Evolution")]
    public int turnsToLive = 0;
    public int turnsToEvolve = 0;
    public Vegetable.VegetableType evolutionType;

    [Header("Aura Settings")]
    public bool hasAura;         
    public float auraRadius = 2.5f; 

    [Header("Interaction Flags")]
    public bool isStasis;
    public bool isCleanser;
    public bool isInfectious;
    public bool isRadioactive;
    public bool isNegative;
    public bool immuneToNegative;
    public bool immuneToIce;

    [Header("Visual Shake")]
    public bool hasShake;
    public float shakeIntensity = 0.05f;

    [Header("Particle Settings")]
    public int maxParticles = 3;
    public float lifetime = 2.0f;
    public float emissionRate = 1.0f;
    public float size = 50f;
    public float speed = 0.1f;
    public bool rotationEnabled = false;
    public Vector2 rotationSpeed = new Vector2(-1f, 1f);
    public bool disableShape = false;
}
