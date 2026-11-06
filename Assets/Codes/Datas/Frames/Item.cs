using UnityEngine;
using System;

namespace Datas.Frames
{
[Serializable]
public class Item : IData
{
    public int id;
    public string type;
    public float x {get; set;}
    public float y {get; set;}
}
}