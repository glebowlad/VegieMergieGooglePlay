using UnityEngine;

public abstract class EffectAction : ScriptableObject
{
    public abstract void OnImpact(Vegetable self, Collision2D collision);

    public virtual void OnTick(Vegetable self) { }

    public virtual void OnStatusApplied(Vegetable self) { }
}
