using System;
using System.Collections;
using UnityEngine;

public class Merge : MonoBehaviour
{
    public GameObject nextLevelItem;
    private bool isMerging = false;
    private GameObject collidedItem;
    private PrefabPool pool;
    private Vegetable myVeg;
    
    // Событие теперь передает уровень (int)
    public static event Action<int> Merged;

    private void Awake()
    {
        pool = new PrefabPool(nextLevelItem);
        myVeg = GetComponent<Vegetable>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (nextLevelItem == null || isMerging) return;

        Vegetable otherVeg = collision.gameObject.GetComponent<Vegetable>();
        if (otherVeg == null) return;

        bool amIRebirth = (myVeg != null && myVeg.specialType == Vegetable.VegetableType.Reaper);
        bool isSameVeg = collision.gameObject.CompareTag(gameObject.tag);

        if (amIRebirth || isSameVeg)
        {
            Merge otherMerge = collision.gameObject.GetComponent<Merge>();
            if (otherMerge == null || otherMerge.isMerging) return;

            // ИСПРАВЛЕННОЕ УСЛОВИЕ:
            // Если это Жнец эффект — МЫ главные, запускаем сразу.
            // Если это обычное слияние — проверяем ID, как раньше.
            if (amIRebirth || gameObject.GetInstanceID() < collision.gameObject.GetInstanceID())
            {
                isMerging = true;
                otherMerge.isMerging = true;
                collidedItem = collision.gameObject;
                
                if (amIRebirth) myVeg.specialType = Vegetable.VegetableType.Default;

                StartCoroutine(CreateNewItem());
            }
        }
    }



    private IEnumerator CreateNewItem()
    {
        yield return new WaitForSeconds(0.15f);
        GameObject newItem = pool.Get();

        int level = (int)char.GetNumericValue(newItem.name[0]);
        Merged?.Invoke(level);

        newItem.transform.SetParent(transform.parent, false);
        newItem.transform.position = (transform.position + collidedItem.transform.position) / 2f;

        pool.Release(collidedItem);
        pool.Release(gameObject);
        
        newItem.GetComponentInChildren<ParticleSystem>().Play();
        newItem.GetComponent<Rigidbody2D>().simulated = true;
        isMerging = false;
    }
}
