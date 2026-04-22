using UnityEngine;

[CreateAssetMenu(fileName = "RadiationAction", menuName = "Vegetable Effects/Radiation Logic")]
public class RadiationAction : EffectAction
{
    public override void OnImpact(Vegetable self, Collision2D collision)
    {
        // Берем время жизни из базы (Turns To Evolve = 3 на твоем скрине)
        int turns = self.CurrentEffectData != null ? self.CurrentEffectData.turnsToEvolve : 3;
        
        // Создаем таймер-контроллер
        new RadiationProcess(self, turns);
    }
}

// Вспомогательный класс внутри того же файла
public class RadiationProcess
{
    private Vegetable host;
    private int turnsLeft;

    public RadiationProcess(Vegetable v, int turns)
    {
        host = v;
        turnsLeft = turns;

        ExecutePulse();

        Vegetable.OnVegetableDropped += ExecutePulse;
    }

    private void ExecutePulse()
    {
        // ПРОВЕРКА: Если овощ мерджнулся, удален или сменил тип — САМОЛИКВИДАЦИЯ
        if (host == null || !host.gameObject.activeInHierarchy || host.specialType != Vegetable.VegetableType.Radiation)
        {
            Stop();
            return;
        }

        var data = host.CurrentEffectData;
        float radius = data != null ? data.auraRadius : 2.5f;

        if (EffectManager.Instance != null && data != null && data.auraSprite != null)
        {
            EffectManager.Instance.ShowFlash(host.transform.position, data.auraSprite, data.auraColor, radius, data.animType);
        }

        // 2. ФИЗИКА: Облучение
        Collider2D[] hits = Physics2D.OverlapCircleAll(host.transform.position, radius);
        foreach (var hit in hits)
        {
            Vegetable target = hit.GetComponent<Vegetable>();
            if (target != null && target != host && 
                target.specialType != Vegetable.VegetableType.Ice && 
                target.specialType != Vegetable.VegetableType.Enchanted &&
                target.specialType != Vegetable.VegetableType.Virus &&
                target.specialType != Vegetable.VegetableType.Radiation &&
                target.specialType != Vegetable.VegetableType.Mutant)
            {
                target.SetSpecialType(Vegetable.VegetableType.Warning);
            }
        }

        turnsLeft--;

        // 3. ФИНАЛ: Превращение самого носителя
        if (turnsLeft <= 0)
        {
            Stop();
            host.SetSpecialType(Vegetable.VegetableType.Warning);
        }
    }

    private void Stop()
    {
        // Отписываемся от события — это убивает "призрачный" эффект на месте мерджа
        Vegetable.OnVegetableDropped -= ExecutePulse;
    }
}
