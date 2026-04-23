using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; } // Синглтон для быстрого доступа

    [Header("Настройки Пула")]
    public AuraEffect auraPrefab; // Твой созданный префаб
    private SimplePool<AuraEffect> auraPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Раскомментируйте строку ниже, если менеджер должен жить между всеми сценами
            // DontDestroyOnLoad(gameObject); 

            if (auraPrefab != null)
            {
                auraPool = new SimplePool<AuraEffect>(auraPrefab, 5);
            }
        }
        else
        {
            // Если менеджер уже существует на сцене — уничтожаем дубликат
            Destroy(gameObject);
            return;
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
