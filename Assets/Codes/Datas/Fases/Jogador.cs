using UnityEngine;
using System;

namespace Datas.Fase
{
[Serializable] public class Jogador : IData
{   
    public int tank_id; 
    public float x {get; set;} 
    public float y {get; set;}
    public float angulo, hp, energia; 
}
}