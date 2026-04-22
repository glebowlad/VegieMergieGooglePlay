using UnityEngine;

[CreateAssetMenu(fileName = "MagicAction", menuName = "Vegetable Effects/Magic Logic")]
public class MagicAction : EffectAction
{
    public override void OnImpact(Vegetable self, Collision2D collision)
    {
        var data = self.CurrentEffectData;
        if (data == null || !data.hasAura) return;

        float radius = data.auraRadius;

        if (EffectManager.Instance != null && data.auraSprite != null)
        {
            EffectManager.Instance.ShowFlash(
                self.transform.position,
                data.auraSprite,
                data.auraColor,
                radius,
                data.animType
            );
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(self.transform.position, radius);
        foreach (var hit in hits)
        {
            Vegetable target = hit.GetComponent<Vegetable>();
            if (target == null) continue;
            if (target == self) continue;
            if (target.specialType == Vegetable.VegetableType.Enchanted) continue;

            ApplyEnchanted(target);
        }

        ApplyEnchanted(self);
    }

    private void ApplyEnchanted(Vegetable v)
    {
        if (v.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static)
        {
            v.SetSpecialType(Vegetable.VegetableType.Default);
        }
        v.SetSpecialType(Vegetable.VegetableType.Enchanted);
    }
}

