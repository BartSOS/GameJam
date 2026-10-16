using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tank
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
}

[Serializable]
public class Item
{
    public int id;
    public string type;
    public float x;
    public float y;
}

[Serializable]
public class ProjectileData
{
    public int id;
    public float x;
    public float y;
    public int owner_id;
    public float vx;
    public float vy;
}

[Serializable]
public class HazardData
{
    public int id;
    public string type;
    public float x;
    public float y;
    public float radius;
    public float duration;
    public float timer;
    public float angle;
}

[Serializable]
public class Event
{
    public int source_id;
    public int target_id;
    public string type;
    public float x;
    public float y;
}

[Serializable]
public class FrameData
{
    public int tick;
    public List<Tank> tanks;
    public List<ProjectileData> projectiles;
    public List<HazardData> hazards;
    public List<Item> items;
    public List<Event> events;
}

[Serializable]
public class Config
{
    public float arena_height;
    public float arena_width;
    public int max_ticks;
}

[Serializable]
public class Project
{
    public List<FrameData> frames;
    public Config config;
    public float width;
    public float height;

    public Vector3 tam_engine;

    public static float Div(float x, float y)
    {
        return (y == 0? 0 : x / y);
    }

    public static float regraDeTres(float val, float current, float target)
    {
        return (target*val)/current;
    }

    public static Vector3 regraDeTres(Vector3 val, Vector3 current, Vector3 target)
    {
        Vector3 vector = Vector3.Scale(target, val);

        return new Vector3(Div(vector.x, current.x), Div(vector.y, current.y), Div(vector.z, current.z));
    }

    public static Vector3 regraDeTresMaster(Vector3 transformLocalScale, Vector3 tamSpriteWorld, Vector3 tamSpriteEngine)
    {
        CameraDigital cameraDigital = Camera.main.GetComponent<CameraDigital>();
        Vector3 scaleMax = regraDeTres(tamSpriteEngine, cameraDigital.Tam_Engine, cameraDigital.Tam_Mundo);
        return regraDeTres(transformLocalScale, tamSpriteWorld, scaleMax);
    }

    public void normalizaDates<TData>(
    Func<TData, float> extraiX, Func<TData, float> extraiY,
    Action<TData, float> setX, Action<TData, float> setY, 
    List<TData> datas)
    {
        foreach (TData data in datas)
        {
            float x = regraDeTres(extraiX(data), config.arena_width, width);
            float y = regraDeTres(extraiY(data), config.arena_height, height);

            x -= width / 2;
            y -= height / 2;

            setX(data, x);
            setY(data, y);
        }
    }

    public void NormalizeDates(float width, float height)
    {
        this.width = width;
        this.height = height;
        this.tam_engine = new Vector3(width, height);

        foreach (FrameData frame in frames)
        {
            normalizaDates<Tank> (
                t => t.x,
                t => t.y,
                (t, x) => t.x = x,
                (t, y) => t.y = y,
                frame.tanks
            );

            normalizaDates<ProjectileData> (
                t => t.x,
                t => t.y,
                (t, x) => t.x = x,
                (t, y) => t.y = y,
                frame.projectiles
            );

            normalizaDates<HazardData> (
                t => t.x,
                t => t.y,
                (t, x) => t.x = x,
                (t, y) => t.y = y,
                frame.hazards
            );

            normalizaDates<Item> (
                t => t.x,
                t => t.y,
                (t, x) => t.x = x,
                (t, y) => t.y = y,
                frame.items
            );
        }
    }
}
