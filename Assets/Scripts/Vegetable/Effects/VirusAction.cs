using UnityEngine;

[CreateAssetMenu(fileName = "VirusAction", menuName = "Vegetable Effects/Virus Logic")]
public class VirusAction : EffectAction
{
    public override void OnImpact(Vegetable self, Collision2D collision) { }

    public override void OnStatusApplied(Vegetable self)
    {
        // Если на овоще уже висит контроллер вируса — не дублируем
        if (self.GetComponent<VirusController>() != null) return;

        // Берем время жизни из базы (Turns To Evolve)
        int turns = self.CurrentEffectData != null ? self.CurrentEffectData.turnsToEvolve : 3;
        
        // Вешаем временный скрипт-контроллер прямо на овощ
        var controller = self.gameObject.AddComponent<VirusController>();
        controller.Init(self, turns);
    }
}

// Вспомогательный компонент для управления жизнью вируса
public class VirusController : MonoBehaviour
{
    private Vegetable host;
    private int turnsLeft;

    public void Init(Vegetable v, int turns)
    {
        host = v;
        turnsLeft = turns;
        Vegetable.OnVegetableDropped += Tick;
    }

    // СОБСТВЕННАЯ КОЛЛИЗИЯ: работает 3 хода независимо от isActionReady
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (host == null || host.specialType != Vegetable.VegetableType.Virus) return;

        Vegetable target = collision.gameObject.GetComponent<Vegetable>();
        if (target != null && target != host)
        {
            // СПИСОК ИСКЛЮЧЕНИЙ: Не заражаем Лёд, Магию, Радиацию и уже существующих Мутантов
            bool isProtected = target.specialType == Vegetable.VegetableType.Ice || 
                               target.specialType == Vegetable.VegetableType.Enchanted || 
                               target.specialType == Vegetable.VegetableType.Radiation || 
                               target.specialType == Vegetable.VegetableType.Virus ||
                               target.specialType == Vegetable.VegetableType.Reaper ||  
                               target.specialType == Vegetable.VegetableType.Mutant;

            if (!isProtected)
            {
                // Заражаем! Цель становится Мутантом
                target.SetSpecialType(Vegetable.VegetableType.Mutant);
            }
        }
    }

    private void Tick()
    {
        if (host == null || host.isActionReady) return;
        if (host.specialType != Vegetable.VegetableType.Virus)
        {
            Destroy(this); 
            return; 
        }

        turnsLeft--;

        if (turnsLeft <= 0)
        {
            MakePassive(); 
            Destroy(this); 
        }
    }

    private void MakePassive()
    {
        if (host == null) return;
        host.transform.localScale *= 0.96f;

        Transform maskObj = host.transform.Find("SpecialMask");
        if (maskObj != null && maskObj.TryGetComponent<SpriteRenderer>(out var sr))
        {

            sr.color = new Color(0.796f, 0.631f, 0.259f, 0.35f);
        }

        Transform effectObj = host.transform.Find("SpecialEffect");
        if (effectObj != null && effectObj.TryGetComponent<ParticleSystem>(out var ps))
        {
            var main = ps.main;
            var emission = ps.emission;
            var rot = ps.rotationOverLifetime;

            main.maxParticles = 3;
            main.startLifetime = 2.5f;
            main.startSize = 40f;
            main.startSpeed = 0.1f;
            
            emission.rateOverTime = 1.5f;

            rot.enabled = true;
            rot.z = new ParticleSystem.MinMaxCurve(-0.1f, 0.1f);

            ps.Clear();
            ps.Play();
        }
    }


    private void OnDestroy()
    {
        Vegetable.OnVegetableDropped -= Tick;
    }

}
