using UnityEngine;
using System;

[CreateAssetMenu(fileName = "IceAction", menuName = "Vegetable Effects/Ice Logic")]
public class IceAction : EffectAction
{
    public override void OnImpact(Vegetable self, Collision2D collision)
    {
        var rb = self.GetComponent<Rigidbody2D>();
        if (rb == null || rb.bodyType == RigidbodyType2D.Static) return;

        // Замораживаем себя
        Freeze(self);

        // Аура заморозки соседей
        float radius = self.CurrentEffectData != null ? self.CurrentEffectData.auraRadius : 2.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(self.transform.position, radius);
        foreach (var hit in hits)
        {
            Vegetable target = hit.GetComponent<Vegetable>();
            if (target != null && target != self) Freeze(target);
        }
    }

    private void Freeze(Vegetable v)
    {
        var rb = v.GetComponent<Rigidbody2D>();
        if (rb != null && rb.bodyType != RigidbodyType2D.Static)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        v.SetSpecialType(Vegetable.VegetableType.Ice);

        if (v.currentTimer is IceTimer existingTimer)
        {
            existingTimer.ResetTimer(5); // Сбрасываем счетчик обратно на 5
        }
        else
        {
            // Создаем новый и записываем ссылку в овощ
            v.currentTimer = new IceTimer(v, 5);
        }
    }
}

// Вспомогательный класс-счетчик (живет в памяти, пока овощ лед)
public class IceTimer
{
    private Vegetable target;
    private int remainingTurns;

    public IceTimer(Vegetable v, int turns)
    {
        target = v;
        remainingTurns = turns;
        Vegetable.OnVegetableDropped += Tick;
    }
    public void ResetTimer(int turns)
    {
        remainingTurns = turns;
    }

    private void Tick()
    {
        if (target == null || target.specialType != Vegetable.VegetableType.Ice)
        {
            Vegetable.OnVegetableDropped -= Tick;
            if (target != null) target.currentTimer = null; 
            return;
        }

        remainingTurns--;

        if (remainingTurns <= 0)
        {
            Vegetable.OnVegetableDropped -= Tick; // Отписываемся
            target.currentTimer = null; 
            target.SetSpecialType(Vegetable.VegetableType.Default); // Размораживаем
        }
    }
}
