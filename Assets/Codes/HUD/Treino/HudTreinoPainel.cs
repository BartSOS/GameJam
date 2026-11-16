using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Datas.Fase;
using Datas.Frames;

namespace HUD.Treino{
/// <summary>
/// Painel do treino: a parte fixa (etiqueta da fase, título, frame e barra do frame)
/// e, embaixo, o prefab do objetivo da fase.
/// "TREINO", "FRAME" e as cores da parte fixa ficam no prefab, não no código.
/// Uso: Configurar(fase) quando o replay carrega; MostrarFrame(indice) a cada frame.
/// </summary>
public class HudTreinoPainel : MonoBehaviour
{
    [Header("Cabeçalho")]
    [Tooltip("Texto da etiqueta verde. O código escreve \"FASE 02\".")]
    public TMP_Text textoEtiquetaFase;
    [Tooltip("Título da fase (\"Ficar no ponto B\").")]
    public TMP_Text textoTitulo;

    [Header("Linha do frame")]
    [Tooltip("Número grande do frame atual (\"105\").")]
    public TMP_Text textoFrameAtual;
    [Tooltip("Prazo em cinza (\"/ 360\"). Some quando a fase não tem prazo.")]
    public TMP_Text textoFrameLimite;
    [Tooltip("Preenchimento da barra: filho da trilha, Anchor Min (0,0), Anchor Max (0,1), offsets 0. O código muda o Anchor Max X.")]
    public RectTransform barraFramePreenchimento;
    [Tooltip("Image do preenchimento da barra do frame, para ficar laranja perto do prazo. Opcional.")]
    public Image barraFrameImagem;
    public Color corBarraFrame = new Color32(0xE6, 0xED, 0xF3, 0x8C);  // texto a 55%
    public Color corBarraPrazo = new Color32(0xFF, 0x9F, 0x43, 0xFF);  // laranja
    [Tooltip("A partir desta fração do prazo a barra fica laranja.")]
    public float limiarPrazo = 0.8f;

    [Header("Parte de baixo (prefab por objetivo)")]
    [Tooltip("Onde o prefab do objetivo é criado (de preferência com Vertical Layout Group).")]
    public RectTransform areaObjetivo;
    public List<HudTreinoObjetivoPorFase> objetivos = new List<HudTreinoObjetivoPorFase>();

    [Header("Fase concluída (opcional)")]
    [Tooltip("Faixa \"FASE CONCLUÍDA\". Aparece quando o progresso do frame diz concluido.")]
    public GameObject faixaConcluida;

    [Header("Fase não concluída (opcional)")]
    [Tooltip("Faixa laranja \"PRAZO ESGOTADO\": o frame passou do limite_frame sem concluir.")]
    public GameObject faixaPrazoEsgotado;
    [Tooltip("Faixa vermelha \"TANQUE DESTRUÍDO\": o tanque da equipe morreu. Precisa dos frames: Configurar(fase, frames).")]
    public GameObject faixaDestruido;

    private Fases fase;
    private List<Frame> frames;
    private HudTreinoObjetivo objetivo;

    /// <summary>Recebe o bloco fase do replay. Sem progresso (torneio, replay antigo), o painel se esconde.</summary>
    public void Configurar(Fases novaFase)
    {
        Configurar(novaFase, null);
    }

    /// <summary>
    /// Como Configurar(fase), mais os frames do replay: as fases 04, 05 e 06 leem deles
    /// alvos vivos, tiros, energia e vida. Sem os frames, essas linhas ficam paradas.
    /// </summary>
    public void Configurar(Fases novaFase, List<Frame> framesDoReplay)
    {
        frames = framesDoReplay;
        fase = novaFase.TemProgresso() ? novaFase : null;
        TrocarObjetivo(null);

        gameObject.SetActive(fase != null);
        if (fase == null) return;

        if (textoEtiquetaFase) textoEtiquetaFase.text = "FASE " + fase.Numero();
        if (textoTitulo) textoTitulo.text = fase.TituloOuId();
        if (textoFrameLimite)
        {
            textoFrameLimite.gameObject.SetActive(fase.TemPrazo());
            textoFrameLimite.text = FormatoHud.Total(fase.limite_frame);
        }

        TrocarObjetivo(PrefabDaFase(fase.Numero()));
        MostrarFrame(0);
    }

    /// <summary>Índice do frame em frames[] (o progresso tem a mesma ordem).</summary>
    public void MostrarFrame(int indice)
    {
        if (fase == null) return;
        var p = fase.ProgressoNoIndice(indice);
        if (p == null) return;

        if (textoFrameAtual) textoFrameAtual.text = p.frame.ToString();
        float proporcao = FormatoHud.Proporcao(p.frame, fase.UltimoFrame());
        if (barraFramePreenchimento) PreencherBarra(proporcao);
        if (barraFrameImagem) barraFrameImagem.color = fase.TemPrazo() && proporcao >= limiarPrazo ? corBarraPrazo : corBarraFrame;
        if (faixaConcluida) faixaConcluida.SetActive(p.concluido);

        Frame frame = null;
        int i = 0;
        if (frames != null && frames.Count > 0)
        {
            i = Mathf.Clamp(indice, 0, frames.Count - 1);
            frame = frames[i];
        }
        MostrarFalha(p, frame);

        if (objetivo)
        {
            objetivo.Mostrar(p);
            if (frame != null) objetivo.MostrarFrame(frame, i);
        }
    }

    // Destruído tem prioridade sobre prazo; nenhuma das duas aparece depois de concluir.
    private void MostrarFalha(Progresso p, Frame frame)
    {
        bool destruido = !p.concluido && frame != null && !frame.TanqueVivo(fase.IdJogador());
        bool prazo = !p.concluido && !destruido && fase.TemPrazo() && p.frame > fase.limite_frame;
        if (faixaDestruido) faixaDestruido.SetActive(destruido);
        if (faixaPrazoEsgotado) faixaPrazoEsgotado.SetActive(prazo);
    }

    private HudTreinoObjetivo PrefabDaFase(string numero)
    {
        foreach (var o in objetivos)
            if (o != null && o.numeroFase == numero) return o.prefab;
        Debug.LogWarning("HudTreinoPainel: nenhum prefab de objetivo para a fase " + numero);
        return null;
    }

    private void TrocarObjetivo(HudTreinoObjetivo prefab)
    {
        if (objetivo) Destroy(objetivo.gameObject);
        objetivo = null;
        if (prefab == null || areaObjetivo == null) return;
        objetivo = Instantiate(prefab, areaObjetivo);
        objetivo.Configurar(fase);
        if (frames != null) objetivo.ConfigurarFrames(frames);
    }

    private void PreencherBarra(float proporcao)
    {
        var max = barraFramePreenchimento.anchorMax;
        max.x = proporcao;
        barraFramePreenchimento.anchorMax = max;
    }
}
}