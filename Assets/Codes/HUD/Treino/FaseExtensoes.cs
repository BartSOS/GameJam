using UnityEngine;
using Datas.Fase;

/// <summary>
/// Leituras do bloco "fase" do replay usadas pelos HUDs do treino.
/// Não altera a classe Fase: só lê campos que existem no JSON
/// (id, titulo, limite_frame, max_ticks, progresso).
/// Se a sua classe tiver outro nome, troque "Fase" aqui.
/// </summary>
public static class FaseExtensoes
{
    /// <summary>Replay de treino com progresso por frame (o torneio e replays antigos não têm).</summary>
    public static bool TemProgresso(this Fases fase)
    {
        return fase != null && !string.IsNullOrEmpty(fase.id)
            && fase.progresso != null && fase.progresso.Count > 0;
    }

    /// <summary>"02" a partir do id "02_parar".</summary>
    public static string Numero(this Fases fase)
    {
        if (string.IsNullOrEmpty(fase.id)) return "";
        int i = fase.id.IndexOf('_');
        return i > 0 ? fase.id.Substring(0, i) : fase.id;
    }

    /// <summary>Título da fase; num replay antigo, sem título, o id.</summary>
    public static string TituloOuId(this Fases fase)
    {
        return string.IsNullOrEmpty(fase.titulo) ? fase.id : fase.titulo;
    }

    /// <summary>A fase tem prazo (limite_frame = -1 quer dizer sem prazo).</summary>
    public static bool TemPrazo(this Fases fase)
    {
        return fase.limite_frame > 0;
    }

    /// <summary>Último frame possível da tentativa: o prazo, ou o max_ticks quando não há prazo.</summary>
    public static int UltimoFrame(this Fases fase)
    {
        return fase.TemPrazo() ? fase.limite_frame : Mathf.Max(1, fase.max_ticks);
    }

    /// <summary>Item do progresso no índice do frame (progresso tem a mesma ordem de frames[]).</summary>
    public static Progresso ProgressoNoIndice(this Fases fase, int indice)
    {
        if (fase.progresso == null || fase.progresso.Count == 0) return null;
        return fase.progresso[Mathf.Clamp(indice, 0, fase.progresso.Count - 1)];
    }

    /// <summary>Frames seguidos exigidos no raio (20 na fase 02); 0 quando o objetivo não exige.</summary>
    public static int FramesExigidos(this Fases fase)
    {
        if (fase.progresso == null) return 0;
        foreach (var p in fase.progresso)
            if (p.precisa > 0) return p.precisa;
        return 0;
    }

    /// <summary>Id do primeiro ponto que aparece no progresso ("B"); "" se a fase não tem ponto.</summary>
    public static string PrimeiroPonto(this Fases fase)
    {
        if (fase.progresso == null) return "";
        foreach (var p in fase.progresso)
            if (p.TemPonto()) return p.ponto;
        return "";
    }
}
