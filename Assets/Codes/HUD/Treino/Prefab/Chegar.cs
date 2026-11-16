using UnityEngine;
using Datas.Fase;
using HUD.Treino.Linha;

namespace HUD.Treino.Prefab{
/// <summary>
/// Parte de baixo da fase 01 (andar até o ponto B): distância com DENTRO/FORA e velocidade.
/// Tudo vem de fase.progresso.
/// </summary>
public class Chegar : HudTreinoObjetivo
{
    public HudLinhaDistancia distancia;
    [Tooltip("Velocidade; o \"/ frame\" fica fixo no prefab.")]
    public HudLinhaValor velocidade;

    public override void Configurar(Fases fase)
    {
        if (distancia) distancia.DefinirPonto(fase.PrimeiroPonto());
    }

    public override void Mostrar(Progresso p)
    {
        if (distancia) distancia.Mostrar(p);
        if (velocidade) velocidade.Mostrar(FormatoHud.Decimal1(p.velocidade));
    }
}
}