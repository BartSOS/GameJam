using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

namespace Pooling{
public abstract class AMonoBehaviourPool<TVisual, TData> : MonoBehaviour where TVisual : MonoBehaviour, IPoolable<TData>
{
    [SerializeField]private GameObject prefab;
    [SerializeField] private int initialSize = 10;
    private readonly Dictionary<int, TVisual> _activesObjects = new Dictionary<int, TVisual>();
    private readonly Queue<TVisual> _pool = new Queue<TVisual>();

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
        _pool.Enqueue(newObj.GetComponent<TVisual>());

        return newObj;
    }

    public TVisual GetPool(int id)
    {
        if (_pool.Count < 0)
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

        if (_activesObjects.ContainsKey(id))
            _activesObjects.Remove(id);

        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
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
    
}
}