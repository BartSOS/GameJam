using System;

namespace Pooling
{
public abstract class APoolAbleMonobehaviour<TData> : APoolAbleBase
{
    public abstract void onEntry(TData data);
    public abstract void onUpdate(TData data);
}
}