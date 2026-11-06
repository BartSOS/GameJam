using UnityEngine;
using System;
using System.Collections.Generic;

namespace Datas.Frames
{
 
[Serializable]
public class Frame
{
    public int tick;
    public List<Tank> tanks;
    public List<Projectile> projectiles;
    public List<Hazard> hazards;
    public List<Item> items;
    public List<Event> events;
}  

}