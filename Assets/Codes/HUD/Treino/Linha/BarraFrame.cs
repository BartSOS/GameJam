using UnityEngine;
using UnityEngine.UI;
using Datas.Fase;

namespace HUD.Treino.Linha{
/// <summary>
/// Linha "Frame ........ 105 / 360" do painel do treino: a barra de HudLinhaBarra com o prazo da fase.
/// O total é o último frame da fase; o "/ 360" some quando a fase não tem prazo
/// e a barra fica laranja a partir de limiarPrazo.
/// </summary>
public class HudLinhaBarraFrame : HudLinhaBarra
{
    [Header("Prazo")]
    [Tooltip("Image do preenchimento da barra, para ficar laranja perto do prazo. Opcional.")]
    public Image barraImagem;
    public Color corBarra = new Color32(0xE6, 0xED, 0xF3, 0x8C);  // texto a 55%
    public Color corPrazo = new Color32(0xFF, 0x9F, 0x43, 0xFF);  // laranja
    [Tooltip("A partir desta fração do prazo a barra fica laranja.")]
    public float limiarPrazo = 0.8f;

    private bool temPrazo;
    private int ultimoFrame = 1;

    public void Configurar(Fases fase)
    {
        temPrazo = fase.TemPrazo();
        ultimoFrame = fase.UltimoFrame();
        DefinirRotulo(RotulosHud.Frame);
        Configurar(ultimoFrame);
        if (textoTotal) textoTotal.gameObject.SetActive(temPrazo);
    }

    public override void Mostrar(int frame)
    {
        base.Mostrar(frame);
        if (barraImagem)
            barraImagem.color = temPrazo && FormatoHud.Proporcao(frame, ultimoFrame) >= limiarPrazo ? corPrazo : corBarra;
    }
}
}
