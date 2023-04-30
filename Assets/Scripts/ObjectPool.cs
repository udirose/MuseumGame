using System.Collections.Generic;
using UnityEngine;

//courtesy of chatppp
public class ObjectPool<T> where T : Component
{
    private T prefab;
    private Queue<T> pool;

    public ObjectPool(T prefab, int initialSize)
    {
        this.prefab = prefab;
        pool = new Queue<T>(initialSize);

        for (int i = 0; i < initialSize; i++)
        {
            T item = Object.Instantiate(prefab);
            item.gameObject.SetActive(false);
            pool.Enqueue(item);
        }
    }

    public T Get()
    {
        if (pool.Count == 0)
        {
            T item = Object.Instantiate(prefab);
            item.gameObject.SetActive(false);
            pool.Enqueue(item);
        }

        T pooledObject = pool.Dequeue();
        pooledObject.gameObject.SetActive(true);
        return pooledObject;
    }

    public void Return(T item)
    {
        item.gameObject.SetActive(false);
        pool.Enqueue(item);
    }
}