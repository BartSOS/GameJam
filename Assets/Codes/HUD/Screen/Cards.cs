using UnityEngine;
using TMPro;
using Datas;

namespace HUD.Screen
{
public class Cards : MonoBehaviour
{
    public TextMeshProUGUI abates;
    public TextMeshProUGUI vida;
    public TextMeshProUGUI dano;
    public TextMeshProUGUI posicao;

    public void InsereDados(Bot eu)
    {
        if (eu == null) gameObject.SetActive(false);

        posicao.text = eu.posicao.ToString() + "°";
        vida.text = ((int)eu.hp).ToString();
        dano.text = ((int)eu.dano_causado).ToString();
        abates.text = eu.kills.ToString();
    }
}
}