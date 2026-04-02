using UnityEngine;

public class Toxic : MonoBehaviour
{
    private int expiresAt;
    private bool isActive = true;

    void Awake()
    {
        // Будет активен в течение 5 бросков с момента появления
        expiresAt = Vegetable.GetTotalDrops() + 5;
    }

    void Update()
    {
        // Если время раздачи вышло — выключаем ядовитость
        if (isActive && Vegetable.GetTotalDrops() >= expiresAt)
        {
            isActive = false;
            // Возвращаем овощу обычный вид (снимаем зеленую маску)
            GetComponent<Vegetable>().SetSpecialType(Vegetable.VegetableType.Default);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;

        Vegetable target = collision.gameObject.GetComponent<Vegetable>();
        if (target != null)
        {
            // Условие: Не во льду, не болеет сейчас и НЕ болел раньше
            if (target.GetComponent<ToxicStatus>() == null && 
                target.GetComponent<FrozenStatus>() == null && 
                !target.hasRecoveredFromToxic)
            {
                target.gameObject.AddComponent<ToxicStatus>().Init();
            }
        }
    }

}
