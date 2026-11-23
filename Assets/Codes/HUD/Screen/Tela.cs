using Datas;
using Datas.Fase;
using TMPro;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HUD.Screen
{
public class Tela : ADependsProject
{
    public TextMeshProUGUI winOrLoseText;
    public TextMeshProUGUI subtitulo;
    public RectTransform layoutPosition;
    public GameObject prefabPosition;
    public Cards cards;

    private List<Bot> bots = new();
    private List<Datas.Tanques> tanques = new();
    private Color corResultado;
    private Resultado resultado;
    private Bot vencedor;
    private Bot eu;

    private bool treino;
    private Progresso progressoFinal;
    private Progresso progressoConclusao;

    protected override void AoAcordar()
    {
        gameObject.SetActive(false);
    }

    protected override UniTask AoCarregarProject(Project project)
    {
        bots = project.bots ?? new List<Bot>();
        tanques = project.tanques ?? new List<Datas.Tanques>();
        eu = project.dono != null ? bots.Find(b => b.id == project.dono.tank_id) : null;
        vencedor = bots.Find(b => b.vencedor);

        LerProgresso(project);

        if (treino) DefineDadosTreino();
        else DefineDadosPartida();

        return UniTask.CompletedTask;
    }

    public void MostraMsgFinal()
    {
        gameObject.SetActive(true);
        winOrLoseText.gameObject.SetActive(true);
    }

    private void LerProgresso(Project project)
    {
        var lista = project.fase?.progresso;
        treino = lista != null && lista.Count > 0;
        if (!treino) return;

        progressoFinal = lista[lista.Count - 1];
        progressoConclusao = lista.Find(p => p.concluido);
    }

    private void DefineDadosTreino()
    {
        bool concluiu = progressoConclusao != null;
        resultado = concluiu ? Resultado.VITORIA : Resultado.DERROTA;
        corResultado = concluiu ? Cores.Vitoria : Cores.Derrota;

        winOrLoseText.text = concluiu ? "CONCLUÍDO" : "FALHOU";
        winOrLoseText.color = corResultado;
        subtitulo.text = SubtituloTreino(concluiu);

        layoutPosition.gameObject.SetActive(false);
        if (cards) cards.InsereDados(eu);
    }

    private string SubtituloTreino(bool concluiu)
    {
        if (concluiu)
            return $"Fase cumprida no frame {progressoConclusao.frame}";

        bool morreu = eu != null ? !eu.vivo : !progressoFinal.TemPonto();
        if (morreu)
            return $"O tanque foi destruído no frame {progressoFinal.frame}";

        if (progressoFinal.precisa > 0)
            return $"O tempo acabou com {progressoFinal.SeguidosAteOLimite()} de {progressoFinal.precisa} frames no ponto {progressoFinal.ponto}";

        return "O tempo acabou antes de cumprir a fase";
    }

    private void DefineDadosPartida()
    {
        layoutPosition.gameObject.SetActive(true);
        decidirResultadoPartida();
        decidirCorResultado();
        decidirTitulo();
        decidirSubtitulo();
        if (cards) cards.InsereDados(eu);
        criaClassificacao();
    }

    private Color encontraCor(Bot bot)
    {
        Datas.Tanques t = tanques.Find(x => x.id == bot.id);
        return t != null ? Cores.Hex(t.cor) : Cores.Texto;
    }

    private Bot quemMeAbateu()
    {
        return eu != null ? bots.Find(b => b.id == eu.abatido_por) : null;
    }

    private void criaClassificacao()
    {
        for (int i = layoutPosition.childCount - 1; i >= 0; i--)
            Destroy(layoutPosition.GetChild(i).gameObject);

        var ordenados = new List<Bot>(bots);
        ordenados.Sort((a, b) => a.posicao.CompareTo(b.posicao));

        foreach (Bot bot in ordenados)
        {
            GameObject obj = Instantiate(prefabPosition, layoutPosition);
            var classificacao = obj.GetComponent<Classificacao>();
            classificacao.PintarLinha(bot == eu, corResultado, bot, encontraCor(bot));
        }
    }

    private void decidirResultadoPartida()
    {
        if (eu == null)
        {
            resultado = Resultado.NEUTRO;
            return;
        }

        int vencedores = bots.FindAll(b => b.posicao == 1).Count;
        if (vencedores != 1)
        {
            resultado = Resultado.EMPATE;
            return;
        }

        resultado = eu.posicao == 1 ? Resultado.VITORIA : Resultado.DERROTA;
    }

    private void decidirSubtitulo()
    {
        if (resultado == Resultado.VITORIA)
            subtitulo.text = $"{eu.nome} foi o último tanque de pé";
        else if (resultado == Resultado.DERROTA)
        {
            Bot algoz = quemMeAbateu();
            subtitulo.text = eu.vivo
                ? $"{vencedor?.nome ?? "Outro tanque"} venceu a partida"
                : algoz != null
                    ? $"{eu.nome} foi abatido por {algoz.nome} no frame {eu.tick_eliminacao}"
                    : $"{eu.nome} foi abatido no frame {eu.tick_eliminacao} devido a {Texto.Causa(eu.causa_eliminacao)}";
        }
        else if (resultado == Resultado.EMPATE)
            subtitulo.text = "O tempo acabou sem um único vencedor";
        else
            subtitulo.text = vencedor != null ? $"{vencedor.nome} venceu a partida" : "Sem vencedor";
    }

    private void decidirTitulo()
    {
        winOrLoseText.text = resultado switch
        {
            Resultado.VITORIA => Texto.Vitoria,
            Resultado.DERROTA => Texto.Derrota,
            Resultado.EMPATE => Texto.Empate,
            _ => Texto.Neutro
        };
        winOrLoseText.color = corResultado;
    }

    private void decidirCorResultado()
    {
        corResultado = resultado switch
        {
            Resultado.VITORIA => Cores.Vitoria,
            Resultado.DERROTA => Cores.Derrota,
            Resultado.EMPATE => Cores.Empate,
            _ => vencedor != null ? Cores.Vitoria : Cores.Texto
        };
    }
}
}