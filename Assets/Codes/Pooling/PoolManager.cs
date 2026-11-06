using UnityEngine;
using System.Collections.Generic;
using System;

namespace Pooling
{
public class PoolManager : MonoBehaviour
{
    public static PoolManager INSTANCE {get; private set;}
    public List<MonoBehaviour> poolings;

    private readonly Dictionary<Type, object> _poolsByComponentType = new Dictionary<Type, object>();
    // private delegate void ExecutaIfExists();

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
            // Descobre dinamicamente qual é o tipo 'TVisual' que o pool gerencia
                
            System.Type poolType = pool.GetType();


            while (poolType != null && poolType.BaseType != null)
            {

                if (poolType.BaseType.IsGenericType && poolType.BaseType.GetGenericTypeDefinition() == typeof(AMonoBehaviourPool<,>))
                {
                    Type componentType = poolType.BaseType.GetGenericArguments()[0];

                    // Registramos o pool usando o tipo do objeto como chave
                    if (!_poolsByComponentType.ContainsKey(componentType))
                    {
                        _poolsByComponentType.Add(componentType, pool);
                    }
                    else
                    {
                        Debug.LogWarning($"[PoolManager] Já existe um pool registrado para o tipo {componentType.Name}");
                    }
                    break;
                }
                poolType = poolType.BaseType;
            }
        }    
    }

    // Método genérico universal para pegar qualquer objeto de qualquer pool
    public TVisual Spawn<TVisual, TData>(int id, TData data, Vector3 position, Quaternion rotation) where TVisual : MonoBehaviour, IPoolable<TData>
    {
        if (TryGetPool<TVisual, TData>(id, out TVisual spawned))
        {
            spawned.transform.SetPositionAndRotation(position, rotation);
            spawned.onEntry(data);
            return spawned;
        }

        Debug.LogError($"Nenhum pool encontrado para o tipo {typeof(TVisual).Name}");
        return null;
    }

    private bool TryGetPool<TVisual, TData>(int id, out TVisual script) where TVisual : MonoBehaviour, IPoolable<TData>
    {
        if (_poolsByComponentType.TryGetValue(typeof(TVisual), out object poolObj))
        {
            AMonoBehaviourPool<TVisual, TData> pool = (AMonoBehaviourPool<TVisual, TData>)poolObj;
            script = pool.GetPool(id);
            return true;
        }

        script = default(TVisual);
        return false;
    }

    // Método universal para devolver o objeto ao pool correto
    public void Despawn<TVisual, TData>(TVisual obj) where TVisual : MonoBehaviour, IPoolable<TData>
    {
        if (_poolsByComponentType.TryGetValue(typeof(TVisual), out object poolObj))
        {
            AMonoBehaviourPool<TVisual, TData> pool = (AMonoBehaviourPool<TVisual, TData>)poolObj;
            obj.onExit();
            pool.SetPool(obj);
        }
    }

    public void Morph<TVisual, TData> (int id, TData data) where TVisual : MonoBehaviour, IPoolable<TData>
    {
        if (_poolsByComponentType.TryGetValue(typeof(TVisual), out object poolObj))
        {
            AMonoBehaviourPool<TVisual, TData> pool = (AMonoBehaviourPool<TVisual, TData>)poolObj;
            if(pool.GetDictionary(id, out TVisual script))
            {
                script.onUpdate(data); 
                return;
            }
            Debug.LogError($"Nenhum objeto encontrado para o tipo {typeof(TVisual).Name}");
        }

        else
        Debug.LogError($"Nenhum pool encontrado para o tipo {typeof(TVisual).Name}");
    }
}

}