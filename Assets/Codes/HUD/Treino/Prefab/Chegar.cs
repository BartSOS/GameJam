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


    [Tooltip("Onde as linhas são criadas, na ordem dos campos. Vazio = este objeto.")]
    public Transform conteudo;
    private bool linhasInstanciadas;

    /// Troca os campos que apontam para prefabs por cópias na cena.
    private void InstanciarLinhas()
    {
        if (linhasInstanciadas) return;
        linhasInstanciadas = true;
        Transform pai = conteudo ? conteudo : transform;
        distancia = HudInstancia.Garantir(distancia, pai);
        velocidade = HudInstancia.Garantir(velocidade, pai);
    }

    void Awake() => InstanciarLinhas();

    public override void Configurar(Fases fase)
    {
        InstanciarLinhas();
        if (distancia) distancia.DefinirPonto(fase.PrimeiroPonto());
    }

    public override void Mostrar(Progresso p)
    {
        if (distancia) distancia.Mostrar(p);
        if (velocidade) velocidade.Mostrar(FormatoHud.Decimal1(p.velocidade));
    }
}
}