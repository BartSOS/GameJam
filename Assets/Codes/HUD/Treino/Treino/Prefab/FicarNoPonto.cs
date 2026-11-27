using UnityEngine;
using Datas.Fase;
using HUD.Treino.Linha;

namespace HUD.Treino.Prefab{
/// <summary>
/// Parte de baixo da fase 02 (ficar no ponto B): distância com DENTRO/FORA,
/// velocidade e "NO RAIO" (seguidos / precisa) com a barra.
/// Tudo vem de fase.progresso.
/// </summary>
public class FicarNoPonto : HudTreinoObjetivo
{
    public HudLinhaDistancia distancia;
    [Tooltip("Velocidade; o \"/ frame\" fica fixo no prefab.")]
    public HudLinhaValor velocidade;
    [Tooltip("Frames seguidos no raio / frames exigidos, com a barra.")]
    public HudLinhaBarra noRaio;

    [Tooltip("Onde as linhas são criadas, na ordem dos campos. Vazio = este objeto.")]
    public Transform conteudo;
    private bool linhasInstanciadas;

    private void InstanciarLinhas()
    {
        if (linhasInstanciadas) return;
        linhasInstanciadas = true;
        Transform pai = conteudo ? conteudo : transform;
        distancia = HudInstancia.Garantir(distancia, pai);
        velocidade = HudInstancia.Garantir(velocidade, pai);
        noRaio = HudInstancia.Garantir(noRaio, pai);
    }

    void Awake() => InstanciarLinhas();

    public override void Configurar(Fases fase)
    {
        InstanciarLinhas();
        if (distancia) distancia.DefinirPonto(fase.PrimeiroPonto());
        if (noRaio)
        {
            noRaio.Configurar(fase.FramesExigidos());
            noRaio.Mostrar(0);
        }
    }

    public override void Mostrar(Progresso p)
    {
        if (distancia) distancia.Mostrar(p);
        if (velocidade) velocidade.Mostrar(FormatoHud.Decimal1(p.velocidade));
        if (noRaio) noRaio.Mostrar(p.SeguidosAteOLimite());
    }
}
}