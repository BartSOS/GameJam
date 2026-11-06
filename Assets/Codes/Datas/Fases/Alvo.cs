using UnityEngine;
using System;

namespace Datas.Fase
{
[Serializable] public class Alvo : IData
{ 
    public string id; 
    public int tank_id; 
    public float x {get; set;}
    public float y {get; set;}
    public float hp, angulo; 
}
}