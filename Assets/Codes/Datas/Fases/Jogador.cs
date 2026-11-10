using UnityEngine;
using System;

namespace Datas.Fase
{
[Serializable] public class Jogador : IData
{   
    public int tank_id; 
    public float x;
    public float y;
    public float angulo, hp, energia; 

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
}
}