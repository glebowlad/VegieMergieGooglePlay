using UnityEngine;
using System.Collections;

public class ToxicStatus : MonoBehaviour
{
    private int stepsRemaining = 3; 
    private int lastDropCount;
    private Vegetable parentVeg;
    private VegetableVisual vis;
    private Color sickColor = new Color(0.6f, 0.9f, 0.6f); // Бледный зеленоватый оттенок

    public void Init()
    {
        parentVeg = GetComponent<Vegetable>();
        vis = GetComponent<VegetableVisual>();

        // ПРОВЕРКА: Если во льду или УЖЕ болел раньше — выходим
        if (GetComponent<FrozenStatus>() != null || parentVeg.hasRecoveredFromToxic) 
        { 
            Destroy(this); 
            return; 
        }

        lastDropCount = Vegetable.GetTotalDrops();

        if (vis != null)
        {
            // 1. Ставим тип Toxic, чтобы летали ЧЕРЕПА
            vis.UpdateVisuals(Vegetable.VegetableType.Toxic, new Color(0.8f, 1f, 0f));
            
            // 2. Выключаем маску, но КРАСИМ сам овощ в болезненный цвет
            if (vis.specialMask != null) vis.specialMask.gameObject.SetActive(false);
            
            var renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var r in renderers) {
                if (r != vis.specialMask) {
                    r.enabled = true;
                    r.color = sickColor; // Слегка зеленеет
                }
            }
        }

        StartCoroutine(ShrinkProcess());
    }

    private IEnumerator ShrinkProcess()
    {
        while (stepsRemaining > 0)
        {
            if (Vegetable.GetTotalDrops() > lastDropCount)
            {
                lastDropCount = Vegetable.GetTotalDrops();
                stepsRemaining--;
                
                // Плавное уменьшение
                Vector3 startScale = transform.localScale;
                Vector3 targetScale = startScale * 0.9f;
                float t = 0;
                while (t < 1) {
                    t += Time.deltaTime * 2f;
                    transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                    yield return null;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }

        // ВЫЗДОРОВЛЕНИЕ
        parentVeg.hasRecoveredFromToxic = true; // Ставим метку иммунитета
        
        if (vis != null) {
            vis.auraParticles.Stop(); // Убираем частицы
            // Оставляем овощ чуть зеленоватым навсегда, чтобы игрок видел "иммунитет"
        }
        
        Destroy(this);
    }
}
