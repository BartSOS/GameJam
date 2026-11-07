using System;
using System.Collections;

namespace Pooling
{
public interface IState <TData>
{
    public void onEntry(TData data);
    public void onUpdate(TData data);
    public void onExit(Action complete);
}
}