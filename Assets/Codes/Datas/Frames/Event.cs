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
    public float x {get; set;}
    public float y {get; set;}
}    

}
