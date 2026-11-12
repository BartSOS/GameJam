using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Pooling
{
public class Pooler : IPool{
    private readonly APoolAbleBase _prefab;
    private readonly Transform _parent;
    private readonly Dictionary<int, APoolAbleBase> _activesObjects = new Dictionary<int, APoolAbleBase>();
    private readonly Queue<APoolAbleBase> _pool = new Queue<APoolAbleBase>();

    public Pooler(APoolAbleBase prefab, Transform pai, int initialSize)
    {
        this._prefab = prefab;
        this._parent = pai;
        ExpandsPool(initialSize);
    }

    private void ExpandsPool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            CreateNewInstance();
        }
    }

    private APoolAbleBase CreateNewInstance()
    {
        APoolAbleBase newObj = Object.Instantiate(_prefab, _parent);
        newObj.gameObject.name = $"{_prefab.name} (Pool)";
        newObj.gameObject.SetActive(false);
        _pool.Enqueue(newObj);
        return newObj;
    }

    public APoolAbleBase GetPool(int id)
    {
        APoolAbleBase obj = _pool.Count > 0 ? _pool.Dequeue() : CreateNewInstance();
        obj.Id = id;
        obj.gameObject.SetActive(true);
        _activesObjects.Add(obj.Id, obj);

        return obj;
    }

    public void SetPool(APoolAbleBase obj)
    {
        int id = obj.Id;
        removeOfDictionary(id);
        guardaInPool(obj);
    }

    public bool GetDictionary(int id, out APoolAbleBase script)
    {
        script = default(APoolAbleBase);

        if(_activesObjects.TryGetValue(id, out APoolAbleBase value))
        {
            script = value;
            return true;    
        }

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
            APoolAbleBase obj = removeOfDictionary(chave);

            if(obj != default(APoolAbleBase))
            {
                obj.onExit(()=>
                {
                    guardaInPool(obj);
                });
            }
        }
    }

    private void guardaInPool(APoolAbleBase obj)
    {
        if (_pool.Contains(obj)) return;

        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

    private APoolAbleBase removeOfDictionary(int id)
    {
        if (_activesObjects.ContainsKey(id))
        {
            APoolAbleBase obj = _activesObjects[id];
            _activesObjects.Remove(id);    
            return obj;
        }

        return default(APoolAbleBase);
    }
}
}