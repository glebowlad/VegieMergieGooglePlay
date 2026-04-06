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

        if (myVeg.specialType == Vegetable.VegetableType.Ice || otherVeg.specialType == Vegetable.VegetableType.Ice)
        {
            return; 
        }

        bool isReaper = (myVeg != null && myVeg.specialType == Vegetable.VegetableType.Reaper);
        bool isSameVeg = collision.gameObject.CompareTag(gameObject.tag);

        if (isReaper || isSameVeg)
        {
            Merge otherMerge = collision.gameObject.GetComponent<Merge>();
            if (otherMerge == null || otherMerge.isMerging) return;

            if (isReaper || gameObject.GetInstanceID() < collision.gameObject.GetInstanceID())
            {
                isMerging = true;
                otherMerge.isMerging = true;
                collidedItem = collision.gameObject;
                
                StartCoroutine(CreateNewItem(isReaper));
            }
        }
    }



    private IEnumerator CreateNewItem(bool isReaperEffect)
    {
        yield return new WaitForSeconds(0.15f);
        if (isReaperEffect)
        {
            var data = myVeg.CurrentEffectData;
            EffectManager.Instance.ShowFlash(transform.position, data.auraSprite, data.auraColor, data.auraRadius);
        }
        else 
        {
            GameObject newItem = pool.Get();
            // ОБЯЗАТЕЛЬНО: Чистим новый уровень овоща
            var newVeg = newItem.GetComponent<Vegetable>();
            if (newVeg != null) newVeg.HardResetForPool();

            int level = (int)char.GetNumericValue(newItem.name[0]);
            Merged?.Invoke(level);

            newItem.transform.SetParent(transform.parent, false);
            newItem.transform.position = (transform.position + collidedItem.transform.position) / 2f;

            newItem.GetComponentInChildren<ParticleSystem>().Play();
            newItem.GetComponent<Rigidbody2D>().simulated = true;
        }
        pool.Release(collidedItem);
        pool.Release(gameObject);
        
        
        isMerging = false;
    }
}
