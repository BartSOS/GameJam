using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Datas;
using Unity.VisualScripting;
using System.Diagnostics;

namespace HUD
{
public class Classificacao : MonoBehaviour
{
    public Image fundo;
    public Image contorno;
    public Image rect;
    public TextMeshProUGUI nome;
    public TextMeshProUGUI posicao;
    public TextMeshProUGUI abates;
    public TextMeshProUGUI vida;
    
    public void PintarLinha(bool destaque, Color corResultado, Bot eu, Color corRect)
    {
        fundo.color    = destaque ? Cores.ComAlpha(corResultado, 0.14f) : Cores.ComAlpha(Cores.Cartao, 0.70f);
        contorno.enabled = destaque;
        contorno.color = Cores.ComAlpha(corResultado, 0.70f);
        posicao.color  = destaque ? corResultado : Cores.TextoApagado;

        rect.color = corRect;

        posicao.text = eu.posicao.ToString() + "°";
        vida.text = ((int)eu.hp).ToString() + " vida";
        nome.text = eu.nome;
        abates.text = eu.kills.ToString() + " abates";
        

    }
}
}