using System;

namespace Pooling
{
[Serializable]
public class PoolConfig
{
    public int initialSize = 5;
    public APoolAbleBase prefab;
}
}