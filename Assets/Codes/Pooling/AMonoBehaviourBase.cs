using Pooling;
using UnityEngine;

public abstract class AMonoBehaviourBase : MonoBehaviour, IPoolable
{
    public abstract int Id { get; set; }
}