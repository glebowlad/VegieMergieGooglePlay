//using UnityEngine;

//[CreateAssetMenu(fileName = "WarningAction", menuName = "Vegetable Effects/Warning Logic")]
//public class WarningAction : EffectAction
//{
//    public override void OnImpact(Vegetable self, Collision2D collision) { }

//    // Тот самый метод, который мы вызвали из SetSpecialType
//    public override void OnStatusApplied(Vegetable self)
//    {
//        float initialScale = self.currentBaseScale;
//        // Подписываемся на дропы
//        Vegetable.OnVegetableDropped += () =>
//        {
//            if (self == null || self.specialType != Vegetable.VegetableType.Warning) return;

//            if (self.dropCount >=3)
//            {
//                // Изначальный размер обычно (1, 1, 1), но берем текущий для расчета
//                float minScale = initialScale * 0.5f; // Порог уменьшения (в 2 раза)
//                float reductionStep = 0.08f;

//                if (self.currentBaseScale > minScale)
//                {
//                    self.currentBaseScale = Mathf.Max(self.currentBaseScale - reductionStep, minScale);

//                    // Принудительно обновляем визуал, чтобы изменения применились немедленно
//                    if (self.CurrentEffectData != null)
//                    {
//                        self.ApplyEffectSettings(self.CurrentEffectData);
//                    }
//                }
//                    //if (Random.value < 0.05f) 
//                    //{
//                    //    self.SetSpecialType(Vegetable.VegetableType.Mutant);
//                    //}
//                }
//        };
//    }
//}
using UnityEngine;

[CreateAssetMenu(fileName = "WarningAction", menuName = "Vegetable Effects/Warning Logic")]
public class WarningAction : EffectAction
{
    public override void OnImpact(Vegetable self, Collision2D collision) { }

    public override void OnStatusApplied(Vegetable self)
    {
        // Сохраняем начальное значение масштаба при наложении эффекта
        float initialBase = self.currentBaseScale;
        int startCount = Vegetable.dropCount;
        Vegetable.OnVegetableDropped += () =>
        {
            if (self == null || self.specialType != Vegetable.VegetableType.Warning) return;
            int dropsSinceSpawn = Vegetable.dropCount - startCount;
            // Используем >= 3, чтобы на 3-й бросок уже сработало (или > 3 для 4-го)
            if (dropsSinceSpawn >= 3)
            {
                float minLimit = initialBase * 0.5f;
                float reductionStep = 0.08f;

                if (self.currentBaseScale > minLimit)
                {
                    self.currentBaseScale -= reductionStep;
                    if (self.currentBaseScale < minLimit) self.currentBaseScale = minLimit;

                    // ПРЯМОЕ применение масштаба, чтобы перебить расчеты в ApplyEffectSettings
                    float multiplier = (self.CurrentEffectData != null) ? self.CurrentEffectData.scaleMultiplier : 1f;
                    self.transform.localScale = Vector3.one * (self.currentBaseScale * multiplier);

                    
                }
            }
        };
    }
}
