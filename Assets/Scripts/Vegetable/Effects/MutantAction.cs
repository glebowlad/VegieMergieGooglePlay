using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MutantAction", menuName = "Vegetable Effects/Mutant Logic")]
public class MutantAction : EffectAction
{
    public override void OnImpact(Vegetable self, Collision2D collision) { }

    public override void OnStatusApplied(Vegetable self)
    {

        int turns = self.CurrentEffectData != null ? self.CurrentEffectData.turnsToEvolve : 3;

        var existingTimer = self.GetComponent<MutantTimer>();
        if (existingTimer != null)
        {
            existingTimer.ResetTimer(turns); // Если есть — просто обновляем его
        }
        else
        {
            // Если нет — добавляем новый
            self.gameObject.AddComponent<MutantTimer>().Initialize(self, turns);
        }
    }
}

public class MutantTimer: MonoBehaviour
{
    private Vegetable host;
    private int turnsLeft;

    public void Initialize(Vegetable v, int turns)
    {
        host = v;
        turnsLeft = turns;
        Vegetable.OnVegetableDropped += Tick;
    }

    private void Tick()
    {
        if (host == null || host.specialType != Vegetable.VegetableType.Mutant)
        {
            Stop();
            Destroy(this); // Если овощ еще жив, но тип сменился — удаляем компонент таймера
            return;
        }

        if (host.isActionReady) return;

        turnsLeft--;
        if (turnsLeft <= 0)
        {
            Stop();
            host.SetSpecialType(Vegetable.VegetableType.Virus);
        }
    }

    private void Stop()
    {
        Vegetable.OnVegetableDropped -= Tick;
    }
    private void OnDestroy()
    {
        Stop();
    }
    public void ResetTimer(int turns)
    {
        turnsLeft = turns;
    }
}
