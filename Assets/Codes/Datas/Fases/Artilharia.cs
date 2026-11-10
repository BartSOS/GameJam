using UnityEngine;
using System;

namespace Datas.Fase
{    
[Serializable] public class Artilharia : IData
{ 
    public int tick; 
    public float x;
    public float y;
    public float raio; 
    public bool criada; 
    public bool no_jogador; 

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
}
}