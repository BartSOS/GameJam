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


    private bool linhasInstanciadas;

    /// Troca os campos que apontam para prefabs por cópias na cena.
    private void InstanciarLinhas()
    {
        if (linhasInstanciadas) return;
        linhasInstanciadas = true;
        alvos = HudInstancia.Garantir(alvos, transform);
        tiros = HudInstancia.Garantir(tiros, transform);
        energia = HudInstancia.Garantir(energia, transform);
    }

    void Awake() => InstanciarLinhas();

    public override void Configurar(Fases fase)
    {
        InstanciarLinhas();
        idJogador = fase.IdJogador();
        if (alvos) alvos.Configurar(fase);
        if (energia)
        {
            energia.DefinirRotulo(RotulosHud.Energia);
            energia.Configurar(fase.EnergiaInicial());
        }
        if (tiros)
        {
            tiros.DefinirRotulo(RotulosHud.Tiros);
            tiros.Mostrar("0");
        }
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