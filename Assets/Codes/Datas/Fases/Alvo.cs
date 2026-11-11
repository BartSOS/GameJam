using UnityEngine;
using System;

namespace Datas.Fase
{
[Serializable] public class Alvo : IData
{ 
    public string id; 
    public int tank_id; 
    public float x;
    public float y;
    public float hp, angulo; 

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
    int IData.id { get => (int)id[0];}
}
}