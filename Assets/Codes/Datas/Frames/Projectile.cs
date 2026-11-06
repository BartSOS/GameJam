using UnityEngine;
using System;

namespace Datas.Frames
{   
[Serializable]
public class Projectile : IData
{
    public int id;
    public float x {get; set;}
    public float y {get; set;}
    public int owner_id;
    public float vx;
    public float vy;
}
}