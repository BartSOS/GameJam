using UnityEngine;
using System;
using System.Collections.Generic;
using WebCam;
using Datas.Frames;
using Datas.Fase;
using UnityEngine.Video;
using Tanques;

namespace Datas
{
[Serializable]
public class Project
{
    public List<Frames.Frame> frames;
    public List<Bot> bots;
    public List<Tanques> tanques;
    public Dono dono;
    public Config config;
    public Fase.Fases fase;
    public float width;
    public float height;
    public readonly Dictionary<int, string> nomesDosTanques = new();
    public readonly Dictionary<int, Color> corTemaTanque = new();

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
        Debug.LogError($"width: {width}, height: {height}");;
        Debug.LogError($"Config: {config.arena_width}, {config.arena_height}");

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
            normalizeListData(fase.artilharia);
            normalizeUnicData(fase.jogador);            
        }

        else    
            Debug.LogError("Fase ausente do projeto");

        startNamesDosTanques(bots, fase.alvos);
        startColorsDosTanques();
    }

    private void startColorsDosTanques()
    {
        if (tanques == null) Debug.LogError("DEu merda");

        foreach (Tanques tanque in tanques)
        {
            Debug.LogError(tanque.id);
            corTemaTanque[tanque.id] = tanque.cor.FromHex();
        }
    }

    private void startNamesDosTanques(List<Bot> bots, List<Alvo> alvos)
    {
        foreach (var bot in bots)
        {
            nomesDosTanques[bot.id] = bot.nome;
        }

        if (alvos != null)
        {
            foreach (var alvo in alvos)
            {
                nomesDosTanques[alvo.tank_id] = alvo.id; 
            }
        }
    }

    public string GetNomeTank(int id)
    {
        if (nomesDosTanques.TryGetValue(id, out string nome))
            return nome;

        return $"Tank_In_Pool";
    }

    public static void ScaleObject(SpriteRenderer spriteRenderer, Transform transform, Vector3 vector3)
    {
        float max = Mathf.Max(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
        Vector3 tamSpriteBase = new Vector3(max, max, 1f);

        transform.localScale = Project.regraDeTresMaster(
            transform.localScale,
            tamSpriteBase,
            vector3
        );
    }
}
}