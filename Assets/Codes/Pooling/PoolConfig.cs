using System;
using UnityEngine;


namespace Pooling
{
[Serializable]
public class PoolConfig
{
    public int initialSize = 5;
    public APoolAbleBase prefab;
    public Transform parent;
}
}