
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Datas.Fase;

namespace HUD.Treino.Linha{
/// <summary>
/// Linha "DISTÂNCIA A B ........ 84,9 [● DENTRO | ○ FORA]".
/// Mesma lógica da fase 02, como componente reutilizável (fases 01, 03 e 05).
/// </summary>
public class HudLinhaDistancia : MonoBehaviour
{
    [Tooltip("Rótulo; o código escreve \"DISTÂNCIA A \" + id do ponto.")]
    public TMP_Text textoRotulo;
    [Tooltip("Valor (\"84,9\").")]
    public TMP_Text textoValor;
    [Tooltip("Esconde a linha quando não há ponto da vez (tanque destruído).")]
    public bool esconderSemPonto = true;

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

    public void DefinirPonto(string idPonto)
    {
        if (textoRotulo) textoRotulo.text = "DISTÂNCIA A " + idPonto;
    }

    public void Mostrar(Progresso p)
    {
        if (esconderSemPonto) gameObject.SetActive(p.TemPonto());
        if (p.TemPonto()) DefinirPonto(p.ponto);
        if (textoValor) textoValor.text = p.TemPonto() ? FormatoHud.Decimal1(p.distancia) : "—";
        MostrarEtiqueta(p.EstaDentro());
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
}
}