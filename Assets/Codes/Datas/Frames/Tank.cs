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
    public float x;
    public float y;

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
}
}