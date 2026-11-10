using System;
using UnityEngine;

namespace Tiros
{
public class EventsObjectTiroParado : MonoBehaviour
{
    public event Action FimAtirando;    
    public event Action FimExplodindo;

    public void OnFimAtirando() => FimAtirando?.Invoke(); // pra tocar ao fim do clipe Atirando
    public void OnFimExplodindo() => FimExplodindo?.Invoke(); // pra tocar ao fim do clipe Explodindo
}
}