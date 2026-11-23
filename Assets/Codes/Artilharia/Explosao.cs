using System.Collections;
using UnityEngine;
using Datas;
using Datas.Frames;
using System;

namespace Artilharia{
public class Explosao : MonoBehaviour
{
    public event Action FimExplodindo;

    public void OnFimExplodindo() => FimExplodindo?.Invoke(); // pra tocar ao fim do clipe Explodindo
}
}