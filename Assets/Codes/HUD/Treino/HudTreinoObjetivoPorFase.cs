using System;
using UnityEngine;

/// <summary>
/// Liga o número de uma fase ao prefab da parte de baixo do painel.
/// Aparece como item da lista "Objetivos" no Inspector do HudTreinoPainel.
/// </summary>
[Serializable]
public class HudTreinoObjetivoPorFase
{
    [Tooltip("Número da fase, como no início do id: \"02\" para \"02_parar\".")]
    public string numeroFase = "02";
    public HudTreinoObjetivo prefab;
}
