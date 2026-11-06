using UnityEngine;
using System;

namespace Datas.Fase
{
[Serializable] public class Ponto : IData
{ 
    public string id; 
    public int ordem; 
    public float x {get; set;}
    public float y {get; set;}
    public float raio;
    public int consumido_tick;
}
}