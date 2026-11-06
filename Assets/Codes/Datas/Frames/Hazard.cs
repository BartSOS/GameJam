using UnityEngine;
using System;

namespace Datas.Frames
{
[Serializable]
public class Hazard : IData
{
    public int id;
    public string type;
    public float x {get; set;}
    public float y {get; set;}
    public float radius;
    public float duration;
    public float timer;
    public float angle;
}
}