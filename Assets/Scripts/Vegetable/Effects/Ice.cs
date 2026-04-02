using UnityEngine;
using System.Collections;

public class Ice : MonoBehaviour
{
    [Header("Настройки")]
    public float radiusMultiplier = 0.7f; // Твой проверенный радиус
    public int freezeDuration = 5;       // Длительность в ходах

    private bool hasTriggered = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasTriggered) return;

        // Если коснулись любого овоща
        if (collision.gameObject.GetComponent<Vegetable>() != null)
        {
            hasTriggered = true;
            
            // Расчет радиуса (твой вариант через scale)
            float finalRadius = transform.localScale.x * radiusMultiplier;
            
            FreezeArea(finalRadius);
            
            // Текущий ледяной овощ визуально становится обычным льдом
            GetComponent<Vegetable>().SetSpecialType(Vegetable.VegetableType.Default);
        }
    }

    private void FreezeArea(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            Vegetable target = hit.GetComponent<Vegetable>();
            if (target != null && target.gameObject != this.gameObject)
            {
                // Проверяем, нет ли уже заморозки
                FrozenStatus status = target.GetComponent<FrozenStatus>();
                if (status == null) status = target.gameObject.AddComponent<FrozenStatus>();
                
                // Запускаем или обновляем таймер (если прилетела вторая снежинка)
                status.Init(freezeDuration); 
            }
        }
    }


    // Визуальный радиус в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        float r = transform.localScale.x * radiusMultiplier;
        Gizmos.DrawSphere(transform.position, r);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, r);
    }
}
