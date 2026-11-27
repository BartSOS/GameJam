using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Datas.Fase;
using Datas;
using Datas.Frames;
using HUD.Treino.Linha;
using HUD.Treino.Prefab;

namespace HUD.Treino{
/// <summary>
/// Painel do treino: a parte fixa (etiqueta da fase, título e a linha do frame)
/// e, embaixo, o prefab do objetivo da fase.
/// A linha do frame é um prefab (HudLinhaBarraFrame) que o painel instancia.
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
    [Tooltip("Prefab da linha do frame (HudLinhaBarraFrame). Se for prefab, o painel cria uma cópia em areaFrame.")]
    public HudLinhaBarraFrame linhaFrame;
    [Tooltip("Onde a linha do frame é criada. Vazio = este objeto.")]
    public RectTransform areaFrame;

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
    private bool linhaFrameInstanciada;

    void Awake() => InstanciarLinhaFrame();

    /// Troca o campo que aponta para o prefab da linha do frame por uma cópia na cena.
    private void InstanciarLinhaFrame()
    {
        if (linhaFrameInstanciada) return;
        linhaFrameInstanciada = true;
        linhaFrame = HudInstancia.Garantir(linhaFrame, areaFrame ? areaFrame : transform);
    }

    /// <summary>Recebe o bloco fase do replay. Sem progresso (torneio, replay antigo), o painel se esconde.</summary>
    public void Configurar(Fases novaFase)
    {
        Configurar(novaFase, LoadJson.project?.frames);
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
        InstanciarLinhaFrame();
        if (linhaFrame) linhaFrame.Configurar(fase);

        TrocarObjetivo(PrefabDaFase(fase.Numero()));
        MostrarFrame(0);
    }

    /// <summary>Índice do frame em frames[] (o progresso tem a mesma ordem).</summary>
    public void MostrarFrame(int indice)
    {
        if (fase == null) return;
        var p = fase.ProgressoNoIndice(indice);
        if (p == null) return;

        if (linhaFrame) linhaFrame.Mostrar(p.frame);
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
        bool destruido = !p.concluido && frame != null && !FramesHud.TanqueVivo(frame, fase.IdJogador());
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

    public void DesligaHud()
    {
        gameObject.SetActive(false);
    }
}
}