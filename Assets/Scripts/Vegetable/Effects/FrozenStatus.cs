using UnityEngine;
using System.Collections;

public class FrozenStatus : MonoBehaviour
{
    private int unfreezeAt;
    private Rigidbody2D rb;
    private VegetableVisual vis;
    private bool isInitialized = false;

    // Метод для (пере)запуска заморозки
    public void Init(int duration)
    {
        unfreezeAt = Vegetable.GetTotalDrops() + duration;

        // 2. Если овощ только что заморозили впервые
        if (!isInitialized)
        {
            rb = GetComponent<Rigidbody2D>();
            vis = GetComponent<VegetableVisual>();

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            
            if (vis != null)
                vis.UpdateVisuals(Vegetable.VegetableType.Ice);
            
            isInitialized = true;
            // Запускаем корутину только ОДИН раз
            StartCoroutine(CheckTimer());
        }
    }


    private IEnumerator CheckTimer()
    {
        // Пока счетчик дропов не догнал нашу цель - ждем
        while (Vegetable.GetTotalDrops() < unfreezeAt)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // РАЗМОРОЗКА
        if (rb != null) rb.constraints = RigidbodyConstraints2D.None;
        if (vis != null) vis.UpdateVisuals(Vegetable.VegetableType.Default);
        
        // Самоуничтожение компонента (овощ здоров!)
        Destroy(this);
    }
}
