using UnityEngine;
using System;

namespace Datas.Fase
{
[Serializable] public class Ponto : IData
{ 
    public string id; 
    public int ordem; 
    public float x;
    public float y;
    public float raio;
    public int consumido_tick;

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
}
}