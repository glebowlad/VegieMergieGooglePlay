using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance; // Синглтон для быстрого доступа

    [Header("Настройки Пула")]
    public AuraEffect auraPrefab; // Твой созданный префаб
    private SimplePool<AuraEffect> auraPool;

    private void Awake()
    {
        Instance = this;
        // Создаем пул на 5 объектов (этого хватит для вспышек и радиации)
        if (auraPrefab != null)
        {
            auraPool = new SimplePool<AuraEffect>(auraPrefab, 5);
        }
    }

    // Главный метод, который будут вызывать Магия, Лёд или Радиация
    public AuraEffect GetAura()
    {
        if (auraPool == null) return null;
        
        var effect = auraPool.Get();
        effect.SetPool(auraPool); 
        return effect;
    }
    public void ShowFlash(Vector3 pos, Sprite sprite, Color color, float radius, AuraAnimType animType)
    {
        var effect = GetAura();
        if (effect != null)
        {
            effect.transform.position = pos;
            effect.PlayFlash(sprite, color, radius, animType);
        }
    }
}
