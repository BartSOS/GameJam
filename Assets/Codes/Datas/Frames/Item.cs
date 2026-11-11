using UnityEngine;
using System;

namespace Datas.Frames
{
[Serializable]
public class Item : IData
{
    public int id;
    public string type;
    public float x;
    public float y;

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
    int IData.id { get => id;}

}
}