using UnityEngine;
using UnityEngine.Pool;

public class PrefabPool 
{
    private ObjectPool<GameObject> _pool;
    private GameObject _singlePrefab;
    private GameObject[] _prefabArray;
    private System.Random _random;


    public PrefabPool(GameObject prefab)
    {
        
       _singlePrefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreatePrefab, OnGetPrefab, OnReleasePrefab, OnPrefabDestroy, false);
    }
    public PrefabPool(GameObject[] prefabs, int prewarmObjectsCountPerPrefab)
    {
        _prefabArray = prefabs;
        _random = new System.Random();
        int totalPrewarmCount = prewarmObjectsCountPerPrefab * prefabs.Length;

        _pool = new ObjectPool<GameObject>(OnCreateRandomPrefab, OnGetPrefab, OnReleasePrefab, OnPrefabDestroy, false,
            totalPrewarmCount);
    }


    public GameObject GetRandom()
    {
        var randomObj = _pool.Get();
        return randomObj;
    }


    public GameObject Get()
    {
        var obj = _pool.Get();
        return obj;
    }
    public void Release(GameObject obj)
    {
        _pool.Release(obj);
    }
    private void OnPrefabDestroy(GameObject obj)
    {
        GameObject.Destroy(obj);
    }

    private void OnReleasePrefab(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnGetPrefab(GameObject obj)
    {
        obj.gameObject.SetActive(true);
    }

    private GameObject OnCreatePrefab()
    {
        return GameObject.Instantiate(_singlePrefab);
    }
    private GameObject OnCreateRandomPrefab()
    {
        var randomPrefab = _prefabArray[_random.Next(0, _prefabArray.Length)];
        return GameObject.Instantiate(randomPrefab);
    }
}
