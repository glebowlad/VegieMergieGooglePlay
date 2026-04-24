using UnityEngine;
using System.Collections.Generic;

public class SimplePool<T> where T : Component
{
    private T prefab;
    private Stack<T> inactiveObjects = new Stack<T>();

    public SimplePool(T _prefab, int initialCount)
    {
        prefab = _prefab;
        for (int i = 0; i < initialCount; i++)
        {
            CreateNewObject();
        }
    }

    private T CreateNewObject()
    {
        T newObj = GameObject.Instantiate(prefab);
        newObj.gameObject.SetActive(false);
        inactiveObjects.Push(newObj);
        return newObj;
    }

    public T Get()
    {
        T obj = inactiveObjects.Count > 0 ? inactiveObjects.Pop() : GameObject.Instantiate(prefab);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        inactiveObjects.Push(obj);
    }
}
