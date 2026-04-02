using UnityEngine;

public class Giant : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            // 1. Выключаем авто-массу
            rb.useAutoMass = false;

            // 2. Ставим массу 180 (в 3 раза тяжелее самой тяжелой тыквы)
            rb.mass = 180f; 

            // 3. Увеличиваем гравитацию для эффекта "ядра"
            rb.gravityScale = 3.5f;
            
            
            rb.drag = 0.2f;
            rb.angularDrag = 0.2f;

        }
    }
}
