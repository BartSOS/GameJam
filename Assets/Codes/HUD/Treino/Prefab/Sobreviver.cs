using System.Collections.Generic;
using UnityEngine;
using HUD.Treino.Linha;
using Datas.Fase;
using Datas.Frames;

namespace HUD.Treino.Prefab{

/// <summary>
/// Parte de baixo da fase 06 (sobreviver à artilharia): vida, frames até a próxima bomba
/// e bombas que já caíram.
/// Bombas vêm de fase.artilharia (explodem 30 frames depois do tick); a vida precisa dos frames:
/// painel.Configurar(fase, frames).
/// </summary>
public class HudTreinoSobreviver : HudTreinoObjetivo
{
    public HudLinhaBarra vida;
    [Tooltip("Frames até a próxima explosão; \"—\" quando não há mais.")]
    public HudLinhaValor proximaBomba;
    [Tooltip("Bombas que já explodiram; o complemento vira \"/ N\".")]
    public HudLinhaValor bombas;

    private int idJogador;
    private List<int> explosoes = new List<int>();


    private bool linhasInstanciadas;

    /// Troca os campos que apontam para prefabs por cópias na cena.
    private void InstanciarLinhas()
    {
        if (linhasInstanciadas) return;
        linhasInstanciadas = true;
        vida = HudInstancia.Garantir(vida, transform);
        proximaBomba = HudInstancia.Garantir(proximaBomba, transform);
        bombas = HudInstancia.Garantir(bombas, transform);
    }

    void Awake() => InstanciarLinhas();

    public override void Configurar(Fases fase)
    {
        InstanciarLinhas();
        idJogador = fase.IdJogador();
        explosoes = fase.ExplosoesDaArtilharia();
        if (vida)
        {
            vida.DefinirRotulo(RotulosHud.Vida);
            vida.Configurar(fase.VidaInicial());
            vida.Mostrar(fase.VidaInicial());
        }
        if (proximaBomba) proximaBomba.DefinirRotulo(RotulosHud.ProximaBombaEm);
        if (bombas)
        {
            bombas.DefinirRotulo(RotulosHud.Bombas);
            bombas.DefinirComplemento(FormatoHud.Total(explosoes.Count));
        }
    }

    public override void Mostrar(Progresso p)
    {
        int caidas = 0;
        int proxima = -1;
        foreach (var tick in explosoes)
        {
            if (tick <= p.frame) caidas++;
            else if (proxima < 0) proxima = tick;
        }
        if (bombas) bombas.Mostrar(caidas.ToString());
        if (proximaBomba)
        {
            proximaBomba.Mostrar(proxima >= 0 ? (proxima - p.frame).ToString() : "—");
            proximaBomba.MostrarComplemento(proxima >= 0);
        }
    }

    public override void MostrarFrame(Frame frame, int indice)
    {
        if (vida) vida.Mostrar(Mathf.RoundToInt(frame.VidaDoTanque(idJogador)));
    }
}
}