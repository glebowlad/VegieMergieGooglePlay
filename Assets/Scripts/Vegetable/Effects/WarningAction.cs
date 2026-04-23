using UnityEngine;

[CreateAssetMenu(fileName = "WarningAction", menuName = "Vegetable Effects/Warning Logic")]
public class WarningAction : EffectAction
{
    public override void OnImpact(Vegetable self, Collision2D collision) { }

    public override void OnStatusApplied(Vegetable self)
    {
        // Просто вызываем метод инициализации логики на самом овоще
        self.StartWarningLogic();
    }
}
