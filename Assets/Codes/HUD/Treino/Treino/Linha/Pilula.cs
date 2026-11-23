using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Treino.Linha{
/// <summary>
/// Base das linhas com pílulas: "RÓTULO ........ n / total" e as pílulas embaixo.
/// Entre duas pílulas vai uma linha: verde se a pílula anterior foi concluída, cinza senão.
/// </summary>
public abstract class HudLinhaPilulas : MonoBehaviour
{
    [Tooltip("Número grande (\"2\").")]
    public TMP_Text textoValor;
    [Tooltip("Total em cinza (\"/ 4\").")]
    public TMP_Text textoTotal;
    [Tooltip("Pai das pílulas (Horizontal Layout Group).")]
    public RectTransform areaPilulas;
    [Tooltip("Pílulas já montadas à mão, na ordem. Se ficar vazia, o código clona o modelo.")]
    public List<HudPilula> pilulas = new List<HudPilula>();
    [Tooltip("Uma pílula dentro de areaPilulas, desativada, usada como molde.")]
    public HudPilula modeloPilula;

    [Header("Linhas entre pílulas")]
    [Tooltip("Linhas já montadas à mão (linha i liga pílula i à i+1). Se ficar vazia, o código clona o modelo.")]
    public List<Graphic> linhas = new List<Graphic>();
    [Tooltip("Uma linha (Image) dentro de areaPilulas, desativada, usada como molde.")]
    public Graphic modeloLinha;
    static readonly Color corLinhaAcesa   = new Color32(0x20, 0x55, 0x41, 0xFF);
    static readonly Color corLinhaApagada = new Color32(0x1D, 0x26, 0x32, 0xFF);

    protected void MontarPilulas(List<string> ids)
    {
        if (textoTotal) textoTotal.text = FormatoHud.Total(ids.Count);

        if (pilulas.Count == 0 && modeloPilula != null && areaPilulas != null)
        {
            modeloPilula.gameObject.SetActive(false);
            foreach (var id in ids)
            {
                var pilula = Instantiate(modeloPilula, areaPilulas);
                pilula.name = "Pilula " + id;
                pilula.gameObject.SetActive(true);
                pilulas.Add(pilula);
            }
        }

        for (int i = 0; i < pilulas.Count; i++)
        {
            if (!pilulas[i]) continue;
            pilulas[i].gameObject.SetActive(i < ids.Count);
            if (i < ids.Count) pilulas[i].DefinirTexto(ids[i]);
        }

        MontarLinhas(ids.Count);

        if (areaPilulas) LayoutRebuilder.ForceRebuildLayoutImmediate(areaPilulas);
    }

    void MontarLinhas(int quantidade)
    {
        if (modeloLinha) modeloLinha.gameObject.SetActive(false);

        if (linhas.Count == 0 && modeloLinha != null && areaPilulas != null)
        {
            for (int i = 0; i < pilulas.Count - 1; i++)
            {
                if (!pilulas[i]) { linhas.Add(null); continue; }
                var linha = Instantiate(modeloLinha, areaPilulas);
                linha.name = "Linha " + i;
                linha.transform.SetSiblingIndex(pilulas[i].transform.GetSiblingIndex() + 1);
                linhas.Add(linha);
            }
        }

        for (int i = 0; i < linhas.Count; i++)
        {
            if (!linhas[i]) continue;
            linhas[i].gameObject.SetActive(i + 1 < quantidade);
            PintarLinha(i, false);
        }
    }

    protected void MostrarValor(int valor)
    {
        if (textoValor) textoValor.text = valor.ToString();
    }

    protected void MostrarPilula(int indice, EstadoPilula estado)
    {
        if (indice < 0 || indice >= pilulas.Count || !pilulas[indice]) return;

        pilulas[indice].MostrarEstado(estado);
        PintarLinha(indice, estado == EstadoPilula.Feito);
    }

    void PintarLinha(int i, bool acesa)
    {
        if (i < 0 || i >= linhas.Count || !linhas[i]) return;
        linhas[i].color = acesa ? corLinhaAcesa : corLinhaApagada;
    }
}

}