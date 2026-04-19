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
        if ( isMerging) return;

        Vegetable otherVeg = collision.gameObject.GetComponent<Vegetable>();
        if (otherVeg == null) return;

        if (myVeg.specialType == Vegetable.VegetableType.Ice || otherVeg.specialType == Vegetable.VegetableType.Ice)
        {
            return; 
        }

        bool isReaper = (myVeg != null && myVeg.specialType == Vegetable.VegetableType.Reaper);
        bool otherIsReaper = (otherVeg.specialType == Vegetable.VegetableType.Reaper);
        if (!isReaper && !otherIsReaper && nextLevelItem == null) return;

        bool isSameVeg = collision.gameObject.CompareTag(gameObject.tag);
        if(isReaper|| otherIsReaper)
        {
            Merge otherMerge = collision.gameObject.GetComponent<Merge>();
            if (otherMerge == null || otherMerge.isMerging) return;

            if (gameObject.GetInstanceID() < collision.gameObject.GetInstanceID())
            {
                isMerging = true;
                otherMerge.isMerging = true;
                collidedItem = collision.gameObject;
                Vegetable reaperVeg = isReaper ? myVeg : otherVeg;
                StartCoroutine(ReaperRoutine(reaperVeg));
            }
            return;
        }
        if ( isSameVeg)
        {
            Merge otherMerge = collision.gameObject.GetComponent<Merge>();
            if (otherMerge == null || otherMerge.isMerging) return;

            if ( gameObject.GetInstanceID() < collision.gameObject.GetInstanceID())
            {
                isMerging = true;
                otherMerge.isMerging = true;
                collidedItem = collision.gameObject;
                
                StartCoroutine(CreateNewItem());
            }
        }
    }


    private IEnumerator ReaperRoutine(Vegetable sourceVeg)
    {
        yield return new WaitForSeconds(0.15f);
       
            var data = sourceVeg.CurrentEffectData;
            EffectManager.Instance.ShowFlash(transform.position, data.auraSprite, data.auraColor, data.auraRadius, data.animType);
            AudioManager.Instance.PlayEffectSound(data.effectSound);
            pool.Release(collidedItem);
            pool.Release(gameObject);
            isMerging = false;

    }
    private IEnumerator CreateNewItem()
    {
        yield return new WaitForSeconds(0.15f);
        //if (isReaperEffect)
        //{
        //    var data = myVeg.CurrentEffectData;
        //    EffectManager.Instance.ShowFlash(transform.position, data.auraSprite, data.auraColor, data.auraRadius, data.animType);
        //    AudioManager.Instance.PlayEffectSound(data.effectSound);
        //}
        //else
        //{
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
       // }
        pool.Release(collidedItem);
        pool.Release(gameObject);
        isMerging = false;
    }
}
