using UnityEngine;
using System.Collections.Generic;

namespace Pooling{
public abstract class AMonoBehaviourPool<T> : AMonoBehaviourBase where T : MonoBehaviour, IPoolable
{
    [SerializeField]private GameObject prefab;
    [SerializeField] private int initialSize = 10;
    private readonly Dictionary<int, T> _activesObjects = new Dictionary<int, T>();
    private readonly Queue<T> _pool = new Queue<T>();

    protected virtual void Awake()
    {
        ExpandsPool();
    }

    private void ExpandsPool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewInstance();
        }
    }

    private GameObject CreateNewInstance()
    {
        GameObject newObj = Instantiate(prefab);
        newObj.SetActive(false);
        _pool.Enqueue(newObj.GetComponent<T>());

        return newObj;
    }

    public T GetPool()
    {
        if (_pool.Count < 0)
            ExpandsPool();

        T obj = _pool.Dequeue();
        obj.gameObject.SetActive(true);
        _activesObjects.Add(obj.Id, obj);

        return obj;
    }

    public void SetPool(T obj)
    {
        int id = obj.Id;

        if (_activesObjects.ContainsKey(id))
            _activesObjects.Remove(id);

        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
    
}
}