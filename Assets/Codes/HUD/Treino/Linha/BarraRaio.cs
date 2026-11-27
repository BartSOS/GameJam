using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Treino.Linha{
/// <summary>
/// Linha "No raio" (só no prefab Raio): a barra de HudLinhaBarra mais as divisões do total.
/// Com total N, a barra fica dividida em N partes: N-1 divisões, cada uma em i/N da largura de Marcas.
/// As divisões são cópias do modelo, criadas dentro de Marcas.
/// </summary>
public class HudLinhaBarraRaio : HudLinhaBarra
{
    [Header("Divisões")]
    [Tooltip("Objeto Marcas, esticado sobre a barra (Anchor Min X 0, Anchor Max X 1). As divisões são criadas aqui.")]
    public RectTransform marcas;
    [Tooltip("Uma divisão (Marca) dentro de Marcas, usada como molde. O código desativa ela.")]
    public RectTransform modeloDivisao;
    [Tooltip("Largura de cada divisão, em pixels.")]
    public float larguraDivisao = 1f;
    [Tooltip("Acima deste total as divisões não aparecem (ficariam coladas umas nas outras).")]
    public int maximoDivisoes = 60;

    private readonly List<RectTransform> divisoes = new List<RectTransform>();

    public override void Configurar(int valorTotal)
    {
        base.Configurar(valorTotal);
        MontarDivisoes(valorTotal);
    }

    private void MontarDivisoes(int total)
    {
        if (marcas == null || modeloDivisao == null) return;

        // O código posiciona as divisões; um Layout Group em Marcas brigaria com isso.
        var layout = marcas.GetComponent<LayoutGroup>();
        if (layout) layout.enabled = false;

        // Esconde o molde e qualquer marca feita à mão; as cópias são reativadas abaixo.
        foreach (Transform filho in marcas) filho.gameObject.SetActive(false);

        int quantidade = total > 1 && total <= maximoDivisoes ? total - 1 : 0;
        while (divisoes.Count < quantidade)
        {
            var divisao = Instantiate(modeloDivisao, marcas);
            divisao.name = "Divisao " + (divisoes.Count + 1);
            divisoes.Add(divisao);
        }

        for (int i = 0; i < divisoes.Count; i++)
        {
            bool ativa = i < quantidade;
            divisoes[i].gameObject.SetActive(ativa);
            if (ativa) Posicionar(divisoes[i], (float)(i + 1) / total);
        }
    }

    // Âncora X na fração da barra, largura fixa (larguraDivisao) e altura inteira de Marcas.
    private void Posicionar(RectTransform divisao, float x)
    {
        divisao.anchorMin = new Vector2(x, 0f);
        divisao.anchorMax = new Vector2(x, 1f);
        divisao.pivot = new Vector2(0.5f, 0.5f);
        divisao.anchoredPosition = Vector2.zero;
        divisao.sizeDelta = new Vector2(larguraDivisao, 0f);
    }
}
}
