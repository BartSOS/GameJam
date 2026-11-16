using System;
using System.Collections.Generic;
using UnityEngine;

namespace Datas.Fase{
[Serializable] public class Fases {
    public string id; 
    public int seed; 
    public int max_ticks;
    public Jogador jogador;
    public List<Ponto> pontos;
    public List<Alvo> alvos;
    public List<Progresso> progresso = new();
    public List<Artilharia> artilharia;
    public string titulo;
    public int limite_frame = -1;    // prazo da this; -1 = sem prazo

    public bool laser; public bool itens; public bool zona;

    public bool TemProgresso()
    {
        return this != null && !string.IsNullOrEmpty(this.id)
            && this.progresso != null && this.progresso.Count > 0;
    }

    /// <summary>"02" a partir do id "02_parar".</summary>
    public string Numero()
    {
        if (string.IsNullOrEmpty(this.id)) return "";
        int i = this.id.IndexOf('_');
        return i > 0 ? this.id.Substring(0, i) : this.id;
    }

    /// <summary>Título da this; num replay antigo, sem título, o id.</summary>
    public string TituloOuId()
    {
        return string.IsNullOrEmpty(this.titulo) ? this.id : this.titulo;
    }

    /// <summary>A this tem prazo (limite_frame = -1 quer dizer sem prazo).</summary>
    public bool TemPrazo()
    {
        return this.limite_frame > 0;
    }

    /// <summary>Último frame possível da tentativa: o prazo, ou o max_ticks quando não há prazo.</summary>
    public int UltimoFrame()
    {
        return this.TemPrazo() ? this.limite_frame : Mathf.Max(1, this.max_ticks);
    }

    /// <summary>Item do progresso no índice do frame (progresso tem a mesma ordem de frames[]).</summary>
    public Progresso ProgressoNoIndice(int indice)
    {
        if (this.progresso == null || this.progresso.Count == 0) return null;
        return this.progresso[Mathf.Clamp(indice, 0, this.progresso.Count - 1)];
    }

    /// <summary>Frames seguidos exigidos no raio (20 na this 02); 0 quando o objetivo não exige.</summary>
    public int FramesExigidos()
    {
        if (this.progresso == null) return 0;
        foreach (var p in this.progresso)
            if (p.precisa > 0) return p.precisa;
        return 0;
    }

    /// <summary>Id do primeiro ponto que aparece no progresso ("B"); "" se a this não tem ponto.</summary>
    public string PrimeiroPonto()
    {
        if (this.progresso == null) return "";
        foreach (var p in this.progresso)
            if (p.TemPonto()) return p.ponto;
        return "";
    }

    // ------------------------------------------------------------ jogador (this.jogador)

    /// <summary>Id do tanque da equipe em tanques[] e frames[].tanks[] (this.jogador.tank_id).</summary>
    public int IdJogador()
    {
        return this.jogador != null ? this.jogador.tank_id : 1;
    }

    /// <summary>Vida com que a tentativa começou (total da barra de vida).</summary>
    public int VidaInicial()
    {
        return this.jogador != null ? Mathf.RoundToInt(this.jogador.hp) : 100;
    }

    /// <summary>Energia com que a tentativa começou (total da barra de energia).</summary>
    public int EnergiaInicial()
    {
        return this.jogador != null ? Mathf.RoundToInt(this.jogador.energia) : 100;
    }

    // ------------------------------------------------------------ pontos (this.pontos)

    /// <summary>Ids dos pontos na ordem do objetivo (campo "ordem"): P1, P2, P3, P4.</summary>
    public List<string> PontosEmOrdem()
    {
        var ordenados = new List<(int ordem, string id)>();
        if (this.pontos != null)
            foreach (var p in this.pontos) ordenados.Add((p.ordem, p.id));
        ordenados.Sort((a, b) => a.ordem.CompareTo(b.ordem));
        var ids = new List<string>();
        foreach (var o in ordenados) ids.Add(o.id);
        return ids;
    }

    /// <summary>Quantos pontos já contaram até este frame (consumido_tick entre 0 e o frame).</summary>
    public int PontosConsumidosAte(int frame)
    {
        int n = 0;
        if (this.pontos == null) return n;
        foreach (var p in this.pontos)
            if (p.consumido_tick >= 0 && p.consumido_tick <= frame) n++;
        return n;
    }

    // ------------------------------------------------------------ alvos (this.alvos)

    /// <summary>Ids dos alvos (A1, A2...), na ordem do JSON.</summary>
    public List<string> IdsDosAlvos()
    {
        var ids = new List<string>();
        if (this.alvos != null)
            foreach (var a in this.alvos) ids.Add(a.id);
        return ids;
    }

    /// <summary>Id do tanque de cada alvo em frames[].tanks[], na mesma ordem de IdsDosAlvos.</summary>
    public List<int> TanquesDosAlvos()
    {
        var tanques = new List<int>();
        if (this.alvos != null)
            foreach (var a in this.alvos) tanques.Add(a.tank_id);
        return tanques;
    }

    // ------------------------------------------------------------ artilharia (this.artilharia)

    /// <summary>Frames entre o aviso de uma bomba (tick) e a explosão (contrato do replay).</summary>
    public const int FramesAteExplodir = 30;

    /// <summary>Frames em que cada bomba programada explode (só as criadas), em ordem.</summary>
    public List<int> ExplosoesDaArtilharia()
    {
        var explosoes = new List<int>();
        if (this.artilharia != null)
            foreach (var b in this.artilharia)
                if (b.criada) explosoes.Add(b.tick + FramesAteExplodir);
        explosoes.Sort();
        return explosoes;
    }
}
}