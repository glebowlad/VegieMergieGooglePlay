using UnityEngine;

public class EffectDebugUI : MonoBehaviour
{
    // Сюда в инспекторе перетяни объект Spawner из иерархии
    public Spawner spawner; 

    public void ApplyEffect(int typeIndex)
    {
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
                    veg.SetSpecialType((Vegetable.VegetableType)typeIndex);
                }
            }
        }
        
        // Закрываем панель после выбора
        gameObject.SetActive(false);
    }
}
