using System.Collections.Generic;
using UnityEngine;
using Datas.Fase;
using Datas.Frames;

using HUD.Treino.Linha;

namespace HUD.Treino.Prefab{
/// <summary>
/// Parte de baixo da fase 05 (acertar alvos andando): distância a B com DENTRO/FORA,
/// alvos restantes com as pílulas e tiros disparados.
/// A distância vem de fase.progresso; alvos e tiros precisam dos frames: painel.Configurar(fase, frames).
/// </summary>
public class MiraAndando : HudTreinoObjetivo
{
    public HudLinhaDistancia distancia;
    public HudLinhaAlvos alvos;
    [Tooltip("Tiros disparados até o frame.")]
    public HudLinhaValor tiros;

    private int idJogador;
    private int[] tirosAcumulados = new int[0];


    private bool linhasInstanciadas;

    /// Troca os campos que apontam para prefabs por cópias na cena.
    private void InstanciarLinhas()
    {
        if (linhasInstanciadas) return;
        linhasInstanciadas = true;
        distancia = HudInstancia.Garantir(distancia, transform);
        alvos = HudInstancia.Garantir(alvos, transform);
        tiros = HudInstancia.Garantir(tiros, transform);
    }

    void Awake() => InstanciarLinhas();

    public override void Configurar(Fases fase)
    {
        InstanciarLinhas();
        idJogador = fase.IdJogador();
        if (distancia) distancia.DefinirPonto(fase.PrimeiroPonto());
        if (alvos) alvos.Configurar(fase);
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

    public override void Mostrar(Progresso p)
    {
        if (distancia) distancia.Mostrar(p);
    }

    public override void MostrarFrame(Frame frame, int indice)
    {
        if (alvos) alvos.Mostrar(frame);
        if (tiros && indice < tirosAcumulados.Length) tiros.Mostrar(tirosAcumulados[indice].ToString());
    }
}
}