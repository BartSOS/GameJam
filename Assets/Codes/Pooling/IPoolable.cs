using System;
using System.Collections;
using UnityEngine;

namespace Pooling
{
public interface IPoolable <TData>
{
    public int Id {set; get;}

    public Action<TData> onEntry {get; set;}
    public Action<TData> onUpdate {get; set;}
    public Func<IEnumerator> onExit {get; set;}
}
}