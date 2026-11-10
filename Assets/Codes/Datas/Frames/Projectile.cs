using UnityEngine;
using System;

namespace Datas.Frames
{   
[Serializable]
public class Projectile : IData
{
    public int id;
    public float x;
    public float y;
    public int owner_id;
    public float vx;
    public float vy;

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
}
}