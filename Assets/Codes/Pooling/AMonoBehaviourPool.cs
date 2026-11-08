using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

namespace Pooling{
public abstract class AMonoBehaviourPool<TVisual, TData> : MonoBehaviour, IPool where TVisual : MonoBehaviour, IPoolable, IState<TData>
{
    [SerializeField]private GameObject prefab;
    [SerializeField] private int initialSize = 5;
    private readonly Dictionary<int, TVisual> _activesObjects = new Dictionary<int, TVisual>();
    private readonly Queue<TVisual> _pool = new Queue<TVisual>();

    protected virtual void Awake()
    {
        ExpandsPool();
        AwakeChildren();
    }

    protected abstract void AwakeChildren();

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
        _pool.Enqueue(newObj.GetComponent<TVisual>());

        return newObj;
    }

    public TVisual GetPool(int id)
    {
        if (_pool.Count <= 0)
            ExpandsPool();

        TVisual obj = _pool.Dequeue();
        obj.Id = id;
        obj.gameObject.SetActive(true);
        _activesObjects.Add(obj.Id, obj);

        return obj;
    }

    public void SetPool(TVisual obj)
    {
        int id = obj.Id;
        removeOfDictionary(id);
        guardaInPool(obj);
    }

    public bool GetDictionary(int id, out TVisual script)
    {
        script = default(TVisual);

        if(_activesObjects.TryGetValue(id, out TVisual value))
        {
            script = value;
            return true;    
        }

        Debug.Log("Id informado não encontrado");
        return false;
    }

    public void CleanActivesFromDictionary(int tick)
    {
        HashSet<int> idsParaRemover = new HashSet<int>();

        foreach (var item in _activesObjects)
            if (item.Value.Tick < tick)
                idsParaRemover.Add(item.Key);

        foreach (int chave in idsParaRemover)
        {
            TVisual obj = removeOfDictionary(chave);

            if(obj != default(TVisual))
            {
                obj.onExit(()=>
                {
                    guardaInPool(obj);
                });
            }
        }
    }

    private void guardaInPool(TVisual obj)
    {
        if (_pool.Contains(obj)) return;

        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

    private TVisual removeOfDictionary(int id)
    {
        if (_activesObjects.ContainsKey(id))
        {
            TVisual obj = _activesObjects[id];
            _activesObjects.Remove(id);    
            return obj;
        }

        return default(TVisual);
    }
    
}
}