using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Datas.Fase;

namespace HUD.Treino.Prefab{
/// <summary>
/// Parte de baixo da fase 02 (ficar no ponto B): distância com DENTRO/FORA,
/// velocidade e "NO RAIO" (seguidos / precisa) com a barra.
/// Os rótulos fixos ("VELOCIDADE", "NO RAIO", "/ frame") e as divisões da barra ficam no prefab.
/// As cores daqui são as que mudam com o estado.
/// </summary>
public class FicarNoPonto : HudTreinoObjetivo
{
    [Header("Distância")]
    [Tooltip("Rótulo; o código escreve \"DISTÂNCIA A \" + id do ponto.")]
    public TMP_Text textoRotuloDistancia;
    [Tooltip("Valor (\"84,9\").")]
    public TMP_Text textoDistancia;

    [Header("Etiqueta DENTRO / FORA")]
    public Image fundoEtiqueta;
    public Image bolinhaEtiqueta;
    public TMP_Text textoEtiqueta;
    public Sprite spriteCirculoCheio;    // ui_circulo
    public Sprite spriteCirculoContorno; // ui_circulo_contorno
    public Color corDentro = new Color32(0x3D, 0xDC, 0x84, 0xFF);
    public Color fundoDentro = new Color32(0x3D, 0xDC, 0x84, 0x24); // verde a 14%
    public Color corFora = new Color32(0x7D, 0x8B, 0x99, 0xFF);
    public Color fundoFora = new Color32(0x26, 0x32, 0x41, 0xFF);

    [Header("Velocidade")]
    [Tooltip("Valor (\"5,0\"); o \"/ frame\" fica fixo no prefab.")]
    public TMP_Text textoVelocidade;

    [Header("No raio")]
    [Tooltip("Número grande (\"11\").")]
    public TMP_Text textoSeguidos;
    [Tooltip("Total em cinza (\"/ 20\").")]
    public TMP_Text textoPrecisa;
    [Tooltip("Preenchimento da barra: filho da trilha, Anchor Min (0,0), Anchor Max (0,1), offsets 0. O código muda o Anchor Max X. As divisões ficam por cima, fixas.")]
    public RectTransform barraNoRaioPreenchimento;

    private int precisa;

    public override void Configurar(Fases fase)
    {
        precisa = fase.FramesExigidos();
        EscreverRotuloDistancia(fase.PrimeiroPonto());
        if (textoPrecisa) textoPrecisa.text = FormatoHud.Total(precisa);
    }

    public override void Mostrar(Progresso p)
    {
        if (p.TemPonto()) EscreverRotuloDistancia(p.ponto);
        if (textoDistancia) textoDistancia.text = p.TemPonto() ? FormatoHud.Decimal1(p.distancia) : "—";
        MostrarEtiqueta(p.EstaDentro());
        if (textoVelocidade) textoVelocidade.text = FormatoHud.Decimal1(p.velocidade);
        MostrarNoRaio(p.SeguidosAteOLimite());
    }

    private void EscreverRotuloDistancia(string idPonto)
    {
        if (textoRotuloDistancia) textoRotuloDistancia.text = "DISTÂNCIA A " + idPonto;
    }

    private void MostrarEtiqueta(bool dentro)
    {
        Color cor = dentro ? corDentro : corFora;
        if (fundoEtiqueta) fundoEtiqueta.color = dentro ? fundoDentro : fundoFora;
        if (textoEtiqueta)
        {
            textoEtiqueta.text = dentro ? "DENTRO" : "FORA";
            textoEtiqueta.color = cor;
        }
        if (bolinhaEtiqueta)
        {
            var sprite = dentro ? spriteCirculoCheio : spriteCirculoContorno;
            if (sprite) bolinhaEtiqueta.sprite = sprite;
            bolinhaEtiqueta.color = cor;
        }
    }

    private void MostrarNoRaio(int seguidos)
    {
        if (textoSeguidos) textoSeguidos.text = seguidos.ToString();
        if (barraNoRaioPreenchimento)
        {
            var max = barraNoRaioPreenchimento.anchorMax;
            max.x = FormatoHud.Proporcao(seguidos, precisa);
            barraNoRaioPreenchimento.anchorMax = max;
        }
    }
}
}