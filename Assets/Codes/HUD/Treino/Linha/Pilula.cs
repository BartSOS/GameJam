using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HUD.Treino.Linha{
/// <summary>
/// Base das linhas com pílulas: "RÓTULO ........ n / total" e as pílulas embaixo.
/// As pílulas podem ser montadas à mão (lista) ou clonadas do modelo, uma por id.
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
    }

    protected void MostrarValor(int valor)
    {
        if (textoValor) textoValor.text = valor.ToString();
    }

    protected void MostrarPilula(int indice, EstadoPilula estado)
    {
        if (indice < pilulas.Count && pilulas[indice]) pilulas[indice].MostrarEstado(estado);
    }
}

}

