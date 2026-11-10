using UnityEngine;
using System;

namespace Datas.Frames
{
[Serializable]
public class Hazard : IData
{
    public int id;
    public string type;
    public float x;
    public float y;
    public float radius;
    public float duration;
    public float timer;
    public float angle;

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
}
}