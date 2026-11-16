using UnityEngine;
using TMPro;
using Datas;
using Pooling;
using Datas.Fase;
using System;

public class CheckPoint : APoolAbleMonobehaviour<Ponto>
{
    private TextMeshProUGUI texto;

    SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        texto = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void onEntry(Ponto ponto)
    {
        transform.position = new Vector3(ponto.x, ponto.y, 0);
        Project.ScaleObject(
            spriteRenderer,
            transform,
            new Vector3(ponto.raio * 2, ponto.raio * 2, 1f)
        );

        texto.text = ponto.ordem.ToString();
    }

    public override void onUpdate(Ponto data)
    {
        
    }

    public override void onExit(Action complete)
    {
        complete?.Invoke();
    }
}