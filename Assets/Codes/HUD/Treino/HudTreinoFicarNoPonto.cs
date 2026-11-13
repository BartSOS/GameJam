using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Datas.Fase;

/// <summary>
/// Parte de baixo da fase 02 (ficar no ponto B): distância com DENTRO/FORA,
/// velocidade e "NO RAIO" (seguidos / precisa) com os segmentos.
/// Os rótulos fixos ("VELOCIDADE", "NO RAIO", "/ frame") ficam no prefab.
/// As cores daqui são as que mudam com o estado.
/// </summary>
public class HudTreinoFicarNoPonto : HudTreinoObjetivo
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
    [Tooltip("Pai dos segmentos (Horizontal Layout Group).")]
    public RectTransform areaSegmentos;
    [Tooltip("Segmentos já montados à mão. Se ficar vazio, o código clona o modelo abaixo.")]
    public List<Image> segmentos = new List<Image>();
    [Tooltip("Um segmento dentro de areaSegmentos, desativado, usado como molde.")]
    public Image modeloSegmento;
    public Color segmentoAceso = new Color32(0x3D, 0xDC, 0x84, 0xFF);
    public Color segmentoApagado = new Color32(0x26, 0x32, 0x41, 0xFF);

    public override void Configurar(Fases fase)
    {
        int precisa = fase.FramesExigidos();
        EscreverRotuloDistancia(fase.PrimeiroPonto());
        if (textoPrecisa) textoPrecisa.text = FormatoHud.Total(precisa);
        MontarSegmentos(precisa);
    }

    public override void Mostrar(Progresso p)
    {
        if (p.TemPonto()) EscreverRotuloDistancia(p.ponto);
        if (textoDistancia) textoDistancia.text = p.TemPonto() ? FormatoHud.Decimal1(p.distancia) : "–";
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
        for (int i = 0; i < segmentos.Count; i++)
            if (segmentos[i]) segmentos[i].color = i < seguidos ? segmentoAceso : segmentoApagado;
    }

    private void MontarSegmentos(int quantidade)
    {
        if (segmentos.Count > 0 || modeloSegmento == null || areaSegmentos == null) return;
        modeloSegmento.gameObject.SetActive(false);
        for (int i = 0; i < quantidade; i++)
        {
            var seg = Instantiate(modeloSegmento, areaSegmentos);
            seg.name = "Segmento " + (i + 1);
            seg.gameObject.SetActive(true);
            seg.color = segmentoApagado;
            segmentos.Add(seg);
        }
    }
}
