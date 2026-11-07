using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Tanques
{

public class Bar
{
    private Image image;
    private float valorAtual;
    private float larguraMaxima;
    private float altura;

    public Bar(Image image)
    {
        this.image = image;
        this.larguraMaxima = image.rectTransform.sizeDelta.x;
        this.altura = image.rectTransform.sizeDelta.y;
        this.valorAtual = 100f;
    }

    public void updateBar(float val)
    {
        if (Mathf.Approximately(val, valorAtual)) return;

        valorAtual = val;
        image.rectTransform.DOKill();
        image.rectTransform.DOSizeDelta(new Vector2(larguraMaxima * (val / 100f), altura), 0.5f);
    }

    public void setBar(float val)
    {
        valorAtual = val;
        image.rectTransform.sizeDelta = new Vector2(larguraMaxima * (val / 100f), altura);
    }
}
}