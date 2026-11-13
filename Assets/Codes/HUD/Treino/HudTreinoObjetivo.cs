using UnityEngine;
using Datas.Fase;

/// <summary>
/// Base da parte de baixo do painel do treino (um prefab por objetivo de fase).
/// O HudTreinoPainel cria o prefab, chama Configurar uma vez e Mostrar a cada frame.
/// </summary>
public abstract class HudTreinoObjetivo : MonoBehaviour
{
    /// <summary>Chamado uma vez, quando o replay carrega.</summary>
    public abstract void Configurar(Fases fase);

    /// <summary>Chamado a cada frame do replay.</summary>
    public abstract void Mostrar(Progresso progresso);
}
