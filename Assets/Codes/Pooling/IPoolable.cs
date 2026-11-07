using System;
using System.Collections;
using UnityEngine;

namespace Pooling
{
public interface IPoolable
{
    public int Id {set; get;}
    public int Tick {get; set;}
}
}