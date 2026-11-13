using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Datas.Fase;

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

    [Header("Parte de baixo (prefab por objetivo)")]
    [Tooltip("Onde o prefab do objetivo é criado (de preferência com Vertical Layout Group).")]
    public RectTransform areaObjetivo;
    public List<HudTreinoObjetivoPorFase> objetivos = new List<HudTreinoObjetivoPorFase>();

    [Header("Fase concluída (opcional)")]
    [Tooltip("Faixa \"FASE CONCLUÍDA\". Aparece quando o progresso do frame diz concluido.")]
    public GameObject faixaConcluida;

    private Fases fase;
    private HudTreinoObjetivo objetivo;

    /// <summary>Recebe o bloco fase do replay. Sem progresso (torneio, replay antigo), o painel se esconde.</summary>
    public void Configurar(Fases novaFase)
    {
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
        if (barraFramePreenchimento) PreencherBarra(FormatoHud.Proporcao(p.frame, fase.UltimoFrame()));
        if (faixaConcluida) faixaConcluida.SetActive(p.concluido);
        if (objetivo) objetivo.Mostrar(p);
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
    }

    private void PreencherBarra(float proporcao)
    {
        var max = barraFramePreenchimento.anchorMax;
        max.x = proporcao;
        barraFramePreenchimento.anchorMax = max;
    }
}
