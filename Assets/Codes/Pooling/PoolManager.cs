using UnityEngine;
using System.Collections.Generic;
using System.Data.Common;
using System;

namespace Pooling
{
public class PoolManager : MonoBehaviour
{
    public static PoolManager INSTANCE {get; private set;}
    public List<AMonoBehaviourBase> poolings;

    private readonly Dictionary<(int id, Type type), object> _poolsByComponentType = new Dictionary<(int, Type), object>();
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
            // Descobre dinamicamente qual é o tipo 'T' que o pool gerencia
            if (pool is IPoolable)
            {
                
            System.Type poolType = pool.GetType();
            int poolId = pool.Id;


            while (poolType != null && poolType.BaseType != null)
            {

                if (poolType.BaseType.IsGenericType && poolType.BaseType.GetGenericTypeDefinition() == typeof(AMonoBehaviourPool<>))
                {
                    System.Type componentType = poolType.BaseType.GetGenericArguments()[0];

                    var key = (id: poolId, type: componentType);

                    _poolsByComponentType.Add(key, pool);
                    break;
                }
                poolType = poolType.BaseType;
                }
            }
        }    
    }

    // Método genérico universal para pegar qualquer objeto de qualquer pool
    public T Spawn<T>(int id, Vector3 position, Quaternion rotation) where T : MonoBehaviour, IPoolable
    {
        if (_poolsByComponentType.TryGetValue((id, typeof(T)), out object poolObj))
        {
            AMonoBehaviourPool<T> pool = (AMonoBehaviourPool<T>)poolObj;
            T spawned = pool.GetPool();
            spawned.transform.SetPositionAndRotation(position, rotation);

            return spawned;
        }

        Debug.LogError($"Nenhum pool encontrado para o tipo {typeof(T).Name}");
        return null;
    }

    // Método universal para devolver o objeto ao pool correto
    public void Despawn<T>(T obj) where T : MonoBehaviour, IPoolable
    {
        if (_poolsByComponentType.TryGetValue((obj.Id, typeof(T)), out object poolObj))
        {
            AMonoBehaviourPool<T> pool = (AMonoBehaviourPool<T>)poolObj;
            pool.SetPool(obj);
        }
    }

}

}