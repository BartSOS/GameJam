using UnityEngine;
using Datas.Fase;

/// <summary>
/// Leituras de um item de fase.progresso[] (um frame): ponto, distancia, dentro,
/// seguidos, precisa, velocidade, concluido.
/// Não recalcula o objetivo: só interpreta os valores que o avaliador gravou.
/// Se a sua classe tiver outro nome, troque "ProgressoFase" aqui.
/// </summary>
public static class ProgressoFaseExtensoes
{
    /// <summary>Há ponto da vez neste frame ("" quando a fase não tem ponto ou o tanque morreu).</summary>
    public static bool TemPonto(this Progresso p)
    {
        return !string.IsNullOrEmpty(p.ponto);
    }

    /// <summary>Dentro do raio; sem ponto conta como fora.</summary>
    public static bool EstaDentro(this Progresso p)
    {
        return p.TemPonto() && p.dentro;
    }

    /// <summary>Seguidos para exibir: depois de concluir o JSON continua contando (21, 22...); o HUD para em precisa.</summary>
    public static int SeguidosAteOLimite(this Progresso p)
    {
        return p.precisa > 0 ? Mathf.Min(p.seguidos, p.precisa) : p.seguidos;
    }
}
