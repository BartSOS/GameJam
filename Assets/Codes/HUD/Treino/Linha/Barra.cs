using TMPro;
using UnityEngine;

namespace HUD.Treino.Linha{
/// <summary>
/// Linha "RÓTULO ........ atual / total" com uma barra embaixo (Energia, Vida, No raio).
/// O rótulo vem de RotulosHud: quem usa a linha chama DefinirRotulo.
/// Preenchimento: filho da trilha, Anchor Min (0,0), Anchor Max (0,1), offsets 0; o código muda o Anchor Max X.
/// </summary>
public class HudLinhaBarra : MonoBehaviour
{
    [Tooltip("Rótulo à esquerda; o texto vem de RotulosHud.")]
    public TMP_Text textoRotulo;
    [Tooltip("Número grande (\"80\").")]
    public TMP_Text textoValor;
    [Tooltip("Total em cinza (\"/ 100\").")]
    public TMP_Text textoTotal;
    public RectTransform barraPreenchimento;

    private int total = 1;

    public void DefinirRotulo(string rotulo)
    {
        if (textoRotulo) textoRotulo.text = rotulo;
    }

    public virtual void Configurar(int valorTotal)
    {
        total = Mathf.Max(1, valorTotal);
        if (textoTotal) textoTotal.text = FormatoHud.Total(total);
    }

    public virtual void Mostrar(int atual)
    {
        if (textoValor) textoValor.text = atual.ToString();
        if (barraPreenchimento)
        {
            var max = barraPreenchimento.anchorMax;
            max.x = FormatoHud.Proporcao(atual, total);
            barraPreenchimento.anchorMax = max;
        }
    }
}
}