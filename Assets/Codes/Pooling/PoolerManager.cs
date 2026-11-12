using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

namespace Pooling
{
public class PoolerManager : MonoBehaviour
{
    public static PoolerManager INSTANCE {get; private set;}
    public List<PoolConfig> poolings;

    private readonly Dictionary<Type, Pooler> _poolsByComponentType = new();

    public int TickCurrent {get; set;}

    private void Awake()
    {
        INSTANCE = this;
        RegistraPools();
    }

    private void RegistraPools()
    {
        // Registra automaticamente todos os pools adicionados na lista
        foreach (var pool in poolings)
        {
            _poolsByComponentType.Add(pool.prefab.GetType(), new Pooler(pool.prefab, pool.parent, pool.initialSize));
        }    
    }

    // Método genérico universal para pegar qualquer objeto de qualquer pool
    public TVisual Spawn<TVisual, TData>(int id, TData data) where TVisual : APoolAbleMonobehaviour<TData>
    {
        if (TryGetPool<TVisual>(id, out TVisual spawned))
        {
            spawned.onEntry(data);
            spawned.Tick = TickCurrent;
            return spawned;
        }

        Debug.LogError($"Nenhum pool encontrado para o tipo {typeof(TVisual).Name}");
        return null;
    }

    private bool TryGetPool<TVisual>(int id, out TVisual script) where TVisual : APoolAbleBase
    {
        if (_poolsByComponentType.TryGetValue(typeof(TVisual), out Pooler pooler))
        {
            script = (TVisual)pooler.GetPool(id);
            return true;
        }

        script = default(TVisual);
        return false;
    }

    // Método universal para devolver o objeto ao pool correto
    public void Despawn<TVisual, TData>(TVisual obj) where TVisual : APoolAbleMonobehaviour<TData>
    {
        if (_poolsByComponentType.TryGetValue(typeof(TVisual), out Pooler pooler))
        {
            obj.onExit(() => {
                pooler.SetPool(obj);
            });
        }
    }

    public void Morph<TVisual, TData> (int id, TData data) where TVisual : APoolAbleMonobehaviour<TData>
    {
        if (_poolsByComponentType.TryGetValue(typeof(TVisual), out Pooler pooler))
        {
            if(pooler.GetDictionary(id, out APoolAbleBase saida))
            {
                TVisual script =(TVisual)saida;
                script.onUpdate(data); 
                script.Tick = TickCurrent;
            }

            else{
                TVisual obj = Spawn<TVisual, TData> (id, data);
                obj.onUpdate(data);                
            }

            return;
        }

        else
        Debug.LogError($"Nenhum pool encontrado para o tipo {typeof(TVisual).Name}");
    }

    public TVisual GetVisual<TVisual>(int id) where TVisual : APoolAbleBase
    {
        if (_poolsByComponentType.TryGetValue(typeof(TVisual), out Pooler pooler))
        {
            if(pooler.GetDictionary(id, out APoolAbleBase script))
            {
                return (TVisual)script;
            }
        }

        return default(TVisual);
    }

    public void UpdateTick(int tick)
    {
        foreach (IPool pool in _poolsByComponentType.Values)
        {
            pool.CleanActivesFromDictionary(tick);
        }

        TickCurrent = tick + 1;
    }
}

}