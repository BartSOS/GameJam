using UnityEngine;
using System;

namespace Pooling
{
public abstract class APoolAbleBase : MonoBehaviour
{
    public int Id {get; set;}
    public int Tick {get; set;}
    public abstract void onExit(Action complete);

}
}