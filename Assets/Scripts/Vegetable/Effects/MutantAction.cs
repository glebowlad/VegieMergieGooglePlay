using UnityEngine;

[CreateAssetMenu(fileName = "MutantAction", menuName = "Vegetable Effects/Mutant Logic")]
public class MutantAction : EffectAction
{
    public override void OnImpact(Vegetable self, Collision2D collision) { }

    public override void OnStatusApplied(Vegetable self)
    {

        int turns = self.CurrentEffectData != null ? self.CurrentEffectData.turnsToEvolve : 3;

        new MutantTimer(self, turns);
    }
}

public class MutantTimer
{
    private Vegetable host;
    private int turnsLeft;

    public MutantTimer(Vegetable v, int turns)
    {
        host = v;
        turnsLeft = turns;
        Vegetable.OnVegetableDropped += Tick;
    }

    private void Tick()
    {
        if (host == null || host.isActionReady) return; 
        if (host == null || host.specialType != Vegetable.VegetableType.Mutant)
        {
            Stop();
            return;
        }

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
}
