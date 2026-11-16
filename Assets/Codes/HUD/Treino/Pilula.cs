using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Treino{
/// <summary>
/// Uma pílula (P1, A2...) das linhas de pontos e de alvos.
/// Vai na raiz do prefab/modelo da pílula. As cores de cada estado ficam aqui.
/// </summary>
public class HudPilula : MonoBehaviour
{
    [Tooltip("Preenchimento (ui_circulo, Sliced).")]
    public Image fundo;
    [Tooltip("Contorno (ui_circulo_contorno, Sliced).")]
    public Image contorno;
    public TMP_Text texto;
    [Tooltip("Check do ponto feito (ui_check). Opcional.")]
    public GameObject check;

    [Header("Cores")]
    public Color verde = new Color32(0x3D, 0xDC, 0x84, 0xFF);
    public Color verdeFraco = new Color32(0x3D, 0xDC, 0x84, 0x24); // 14%
    public Color cinza = new Color32(0x7D, 0x8B, 0x99, 0xFF);
    public Color escuro = new Color32(0x26, 0x32, 0x41, 0xFF);
    public Color claro = new Color32(0xE6, 0xED, 0xF3, 0xFF);
    public Color textoSobreVerde = new Color32(0x0B, 0x10, 0x16, 0xFF);

    public void DefinirTexto(string id)
    {
        if (texto) texto.text = id;
    }

    public void MostrarEstado(EstadoPilula estado)
    {
        switch (estado)
        {
            case EstadoPilula.Feito:
                Pintar(verde, verde, textoSobreVerde, true, false);
                break;
            case EstadoPilula.Atual:
                Pintar(verdeFraco, verde, verde, false, false);
                break;
            case EstadoPilula.Vivo:
                Pintar(Color.clear, claro, claro, false, false);
                break;
            case EstadoPilula.Destruido:
                Pintar(escuro, escuro, cinza, false, true);
                break;
            default: // Pendente
                Pintar(Color.clear, escuro, cinza, false, false);
                break;
        }
    }

    private void Pintar(Color corFundo, Color corContorno, Color corTexto, bool comCheck, bool riscado)
    {
        if (fundo) fundo.color = corFundo;
        if (contorno) contorno.color = corContorno;
        if (check) check.SetActive(comCheck);
        if (texto)
        {
            texto.color = corTexto;
            texto.fontStyle = riscado
                ? texto.fontStyle | FontStyles.Strikethrough
                : texto.fontStyle & ~FontStyles.Strikethrough;
        }
    }
}
}