using System.Collections.Generic;
using UnityEngine;
using HUD.Treino.Linha;
using Datas.Frames;
using Datas.Fase;

namespace HUD.Treino.Prefab{
/// <summary>
/// Parte de baixo da fase 04 (acertar alvos parados): alvos restantes com as pílulas,
/// tiros disparados e energia.
/// Precisa dos frames: chame painel.Configurar(fase, frames).
/// </summary>
public class AcertarAlvos : HudTreinoObjetivo
{
    public HudLinhaAlvos alvos;
    [Tooltip("Tiros disparados até o frame.")]
    public HudLinhaValor tiros;
    public HudLinhaBarra energia;

    private int idJogador;
    private int[] tirosAcumulados = new int[0];

    public override void Configurar(Fases fase)
    {
        idJogador = fase.IdJogador();
        if (alvos) alvos.Configurar(fase);
        if (energia) energia.Configurar(fase.EnergiaInicial());
        if (tiros) tiros.Mostrar("0");
    }

    public override void ConfigurarFrames(List<Frame> frames)
    {
        tirosAcumulados = frames.TirosAcumulados(idJogador);
    }

    public override void Mostrar(Progresso p) { }

    public override void MostrarFrame(Frame frame, int indice)
    {
        if (alvos) alvos.Mostrar(frame);
        if (tiros && indice < tirosAcumulados.Length) tiros.Mostrar(tirosAcumulados[indice].ToString());
        if (energia) energia.Mostrar(Mathf.FloorToInt(frame.EnergiaDoTanque(idJogador)));
    }
}
}