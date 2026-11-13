using Datas;
using TMPro;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HUD
{

public class Tela : ADependsProject
{
    private List<Bot> bots;
    private List<Datas.Tanques> tanques;
    public TextMeshProUGUI winOrLoseText;
    public TextMeshProUGUI subtitulo;
    public RectTransform layoutPosition;
    public GameObject prefabPosition;
    public Cards cards;

    private Color corResultado;
    private Resultado resultado;
    private Bot vencedor;

    private Bot eu;
    
    protected override void AoAcordar()
    {
        gameObject.SetActive(false);
    }

    protected override async UniTask AoCarregarProject(Project project)
    {
        this.bots = project.bots;
        achaEu(project);
        DefineDados();
    }

    private Color encontraCor(Bot bot)
    {
        Datas.Tanques t = tanques.Find(t => t.id == bot.id);

        return Cores.Hex(t.cor);
    }

    private void achaEu(Project project)
    {
        if (project.dono == null)
        {
            eu = null;
            return;   
        }

        this.eu = bots.Find(b => b.id == project.dono.tank_id);
        this.tanques = project.tanques;
    }

    private void DefineDados()
    {
        vencedor = bots.Find(b => b.vencedor);
        decidirResultadoPartida();
        decidirCorResultado();
        decidirTitulo();
        decidirSubtitulo();
        criaCards();
        criaClassificacao();
    }

    private Bot quemMeAbateu()
    {
        return bots.Find(b => b.id == eu.abatido_por);
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
            Classificacao classificacao = obj.GetComponent<Classificacao>();
            bool destaque = bot == eu;
            classificacao.PintarLinha(destaque, corResultado, bot, encontraCor(bot));
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
        winOrLoseText.text =  resultado switch
        {
            Resultado.VITORIA => Texto.Vitoria,
            Resultado.DERROTA => Texto.Derrota,
            Resultado.EMPATE => Texto.Empate,
            _ => Texto.Neutro
        };

        winOrLoseText.color = corResultado;

Debug.Log($"{winOrLoseText.text} cor={corResultado}");
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

    private void criaCards()
    {
        cards.InsereDados(eu);
    }

    


    public void MostraMsgFinal()
    {
        gameObject.SetActive(true);
        winOrLoseText.gameObject.SetActive(true);
    }
    
}
}

// "bots": [
//     {
//       "id": 1,
//       "nome": "bot",
//       "lang": "Python 3",
//       "posicao": 1,
//       "vivo": true,
//       "kills": 1,
//       "hp": 100.0,
//       "vencedor": true,
//       "tick_eliminacao": null
//     }