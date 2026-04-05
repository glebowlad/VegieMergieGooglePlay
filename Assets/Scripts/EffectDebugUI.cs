using UnityEngine;

public class EffectDebugUI : MonoBehaviour
{
    // Сюда в инспекторе перетяни объект Spawner из иерархии
    public Spawner spawner; 

    public void ApplyEffect(int typeIndex)
    {
        // 1. Ищем текущий овощ в спавнере
        // Используем Reflection или просто сделаем поле в Spawner публичным (ниже напишу как)
        if (spawner != null)
        {
            // Здесь мы предполагаем, что ты сделал itemToSpawn публичным или добавил свойство
            GameObject currentVegObj = spawner.GetCurrentItem(); 

            if (currentVegObj != null)
            {
                Vegetable veg = currentVegObj.GetComponent<Vegetable>();
                VegetableVisual visual = currentVegObj.GetComponent<VegetableVisual>();

                if (veg != null && visual != null)
                {
                    // Меняем тип по индексу из Enum
                    veg.specialType = (Vegetable.VegetableType)typeIndex;
                    
                    // Обновляем визуал (цвета и частицы подтянутся из наших ассетов!)
                    visual.UpdateVisuals(veg.specialType);
                }
            }
        }
        
        // Закрываем панель после выбора
        gameObject.SetActive(false);
    }
}
