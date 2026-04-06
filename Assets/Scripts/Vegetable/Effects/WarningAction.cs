using UnityEngine;

[CreateAssetMenu(fileName = "WarningAction", menuName = "Vegetable Effects/Warning Logic")]
public class WarningAction : EffectAction
{
    public override void OnImpact(Vegetable self, Collision2D collision) { }

    // Тот самый метод, который мы вызвали из SetSpecialType
    public override void OnStatusApplied(Vegetable self)
    {
        // Подписываемся на дропы
        Vegetable.OnVegetableDropped += () => 
        {
            if (self == null || self.specialType != Vegetable.VegetableType.Warning) return;

            if (Random.value < 0.25f) 
            {
                self.SetSpecialType(Vegetable.VegetableType.Mutant);
            }
        };
    }
}
