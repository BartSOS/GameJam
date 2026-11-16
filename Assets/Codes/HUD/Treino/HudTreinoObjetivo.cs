using System.Collections.Generic;
using UnityEngine;
using Datas.Fase;
using Datas.Frames;  

namespace HUD.Treino{

/// <summary>
/// Base da parte de baixo do painel do treino (um prefab por objetivo de fase).
/// O HudTreinoPainel cria o prefab, chama Configurar uma vez e Mostrar a cada frame.
/// Fases que precisam de frames[] (alvos, tiros, vida, energia) sobrescrevem
/// ConfigurarFrames e MostrarFrame; as outras não precisam fazer nada.
/// </summary>
public abstract class HudTreinoObjetivo : MonoBehaviour
{
    /// <summary>Chamado uma vez, quando o replay carrega.</summary>
    public abstract void Configurar(Fases fase);

    /// <summary>Chamado a cada frame do replay.</summary>
    public abstract void Mostrar(Progresso progresso);

    /// <summary>Chamado uma vez, logo depois de Configurar, quando o painel recebeu os frames do replay.</summary>
    public virtual void ConfigurarFrames(List<Frame> frames) { }

    /// <summary>Chamado a cada frame, depois de Mostrar, com o frame de mesmo índice.</summary>
    public virtual void MostrarFrame(Frame frame, int indice) { }
}
}