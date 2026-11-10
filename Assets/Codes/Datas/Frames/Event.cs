using UnityEngine;
using System;

namespace Datas.Frames
{
[Serializable]
public class Event : IData
{
    public int source_id;
    public int target_id;
    public string type;
    public float x;
    public float y;

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
}    

}
