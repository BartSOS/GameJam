using UnityEngine;
using System;
using System.Collections.Generic;
using WebCam;

namespace Datas
{
[Serializable]
public class Project
{
    public List<Frames.Frame> frames;
    public List<Name2Id> name2Ids;
    public Config config;
    public Fase.Fases fase;
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

    private void normalizeListData(IEnumerable<IData> datas)
    {
        if (datas == null)
        {
            Debug.LogError($"List do tipo {datas.GetType()} não existe");
            return ;
        }

        foreach (IData data in datas)
        {
            normalizeUnicData(data);
        }
    }

    private void normalizeUnicData(IData data)
    {
        if (data == null)
        {
            Debug.LogError($"O tipo {data.GetType()} não existe");
            return ;
        }

        float x = regraDeTres(data.x, config.arena_width, width);
        float y = regraDeTres(data.y, config.arena_height, height);

        x -= width / 2;
        y -= height / 2;

        data.x = x;
        data.y = y;
    }

    public void NormalizeDates(float width, float height)
    {
        this.width = width;
        this.height = height;
        this.tam_engine = new Vector3(width, height);

        if (frames != null)
        {
            foreach (Frames.Frame frame in frames)
            {
                normalizeListData(frame.tanks);
                normalizeListData(frame.projectiles);
                normalizeListData(frame.hazards);
                normalizeListData(frame.items);
            }
        }

        else
            Debug.LogError("Lista de frames ausente do projeto");

        if (fase != null)
        {
            normalizeListData(fase.alvos);
            normalizeListData(fase.pontos);
            normalizeListData(fase.artilharias);
            normalizeUnicData(fase.jogador);
        }

        else    
            Debug.LogError("Fase ausente do projeto");

    }

    public static void ScaleObject(SpriteRenderer spriteRenderer, Transform transform, Vector3 vector3)
    {
        float max = Mathf.Max(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
        Vector3 tamSpriteBase = new Vector3(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y, 1f);

        transform.localScale = Project.regraDeTresMaster(
            transform.localScale,
            tamSpriteBase,
            vector3
        );
    }
}
}