using UnityEngine;
using HUD.Treino.Linha;
using Datas.Fase;

namespace HUD.Treino.Prefab{
/// <summary>
/// Parte de baixo da fase 03 (percorrer pontos em ordem): distância ao ponto da vez,
/// pontos feitos com as pílulas P1..P4 e velocidade.
/// Vem de fase.progresso e de fase.pontos[].consumido_tick.
/// </summary>
public class HudTreinoPercurso : HudTreinoObjetivo
{
    public HudLinhaDistancia distancia;
    public HudLinhaPontos pontos;
    [Tooltip("Velocidade; o \"/ frame\" fica fixo no prefab.")]
    public HudLinhaValor velocidade;

    public override void Configurar(Fases fase)
    {
        if (distancia) distancia.DefinirPonto(fase.PrimeiroPonto());
        if (pontos) pontos.Configurar(fase);
    }

    public override void Mostrar(Progresso p)
    {
        if (distancia) distancia.Mostrar(p);
        if (pontos) pontos.Mostrar(p);
        if (velocidade) velocidade.Mostrar(FormatoHud.Decimal1(p.velocidade));
    }
}
}