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


    private bool linhasInstanciadas;

    /// Troca os campos que apontam para prefabs por cópias na cena.
    private void InstanciarLinhas()
    {
        if (linhasInstanciadas) return;
        linhasInstanciadas = true;
        distancia = HudInstancia.Garantir(distancia, transform);
        velocidade = HudInstancia.Garantir(velocidade, transform);
    }

    void Awake() => InstanciarLinhas();

    public override void Configurar(Fases fase)
    {
        InstanciarLinhas();
        if (distancia) distancia.DefinirPonto(fase.PrimeiroPonto());
        if (velocidade) velocidade.DefinirRotulo(RotulosHud.Velocidade);
    }

    public override void Mostrar(Progresso p)
    {
        if (distancia) distancia.Mostrar(p);
        if (velocidade) velocidade.Mostrar(FormatoHud.Decimal1(p.velocidade));
    }
}
}