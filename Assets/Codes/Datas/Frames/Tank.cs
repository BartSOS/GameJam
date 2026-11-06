using UnityEngine;
using System;

namespace Datas.Frames
{
    
[Serializable]
public class Tank : IData
{
    public bool alive;
    public float angle;
    public float energy;
    public float hp;
    public int id;
    public int kills;
    public bool shield;
    public float turret;
    public float x {get; set;}
    public float y {get; set;}
}
}