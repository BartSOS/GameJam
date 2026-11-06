using UnityEngine;
using System;

namespace Datas.Fase
{    
[Serializable] public class Artilharia : IData
{ 
    public int tick; 
    public float x {get; set;}
    public float y {get; set;}
    public float raio; 
    public bool criada; 
    public bool no_jogador; 
}
}