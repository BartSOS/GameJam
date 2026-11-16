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

    public const string EventoTiro = "SHOOT";

    /// <summary>O tanque está vivo neste this (tanque que não aparece conta como morto).</summary>
    public bool TanqueVivo(int tankId)
    {
        if (this == null || this.tanks == null) return false;
        foreach (var t in this.tanks)
            if (t.id == tankId) return t.alive;
        return false;
    }

    /// <summary>Vida do tanque neste this (nunca negativa; 0 se não aparece).</summary>
    public float VidaDoTanque(int tankId)
    {
        if (this == null || this.tanks == null) return 0f;
        foreach (var t in this.tanks)
            if (t.id == tankId) return t.hp > 0f ? t.hp : 0f;
        return 0f;
    }

    /// <summary>Energia do tanque neste this (0 se não aparece).</summary>
    public float EnergiaDoTanque(int tankId)
    {
        if (this == null || this.tanks == null) return 0f;
        foreach (var t in this.tanks)
            if (t.id == tankId) return t.energy;
        return 0f;
    }

    /// <summary>Tiros que o tanque disparou só neste this (eventos SHOOT dele).</summary>
    public int TirosDoTanque(int tankId)
    {
        int n = 0;
        if (this == null || this.events == null) return n;
        foreach (var e in this.events)
            if (e.type == EventoTiro && e.source_id == tankId) n++;
        return n;
    }

    /// <summary>Tiros acumulados até cada this (índice igual ao de frames[]). Calcule uma vez, ao carregar.</summary>
    
}  

public static class FrameExtensoes{

public static int[] TirosAcumulados(this List<Frame> frames, int tankId)
    {
        var total = new int[frames != null ? frames.Count : 0];
        int soma = 0;
        for (int i = 0; i < total.Length; i++)
        {
            soma += frames[i].TirosDoTanque(tankId);
            total[i] = soma;
        }
        return total;
    }
}
}