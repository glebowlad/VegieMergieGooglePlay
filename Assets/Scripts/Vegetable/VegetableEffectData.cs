using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectData", menuName = "Vegetable/Effect Data")]
public class VegetableEffectData : ScriptableObject
{
    public Vegetable.VegetableType type;
    public Sprite icon;
    public Color maskColor = Color.white;

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
