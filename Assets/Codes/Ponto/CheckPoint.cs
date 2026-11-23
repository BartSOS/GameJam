using UnityEngine;
using TMPro;
using Datas;
using Pooling;
using Datas.Fase;
using System;

namespace Pontos{
/// <summary>
/// Marcador de ponto no campo. Sprites brancos (marcador_ponto / marcador_consumido) tingidos pela cor do SpriteRenderer.
/// Quem tem o frame atual chama MostrarEstado(progresso) a cada frame do replay.
/// </summary>
public class CheckPoint : APoolAbleMonobehaviour<Ponto>
{
    [Header("Sprites (brancos)")]
    public Sprite spritePonto;      // marcador_ponto: Fora e Dentro
    public Sprite spriteConsumido;  // marcador_consumido: com o check

    [Header("Cores")]
    public Color corFora = new Color32(0xE6, 0xED, 0xF3, 0xFF);
    public Color corDentro = new Color32(0x3D, 0xDC, 0x84, 0xFF);
    public Color corConsumido = new Color32(0x3D, 0xDC, 0x84, 0x99); // verde a 60%
    [Tooltip("Opacidade do número quando o ponto já foi consumido.")]
    [Range(0f, 1f)] public float alfaTextoConsumido = 0.6f;

    private Vector3 escalaOriginal;
    private TextMeshProUGUI texto;
    private SpriteRenderer spriteRenderer;
    private Color corTextoOriginal;

    private Ponto ponto;
    private EstadoCheckPoint estado;
    private bool estadoDefinido;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        texto = GetComponentInChildren<TextMeshProUGUI>();
        if (texto) corTextoOriginal = texto.color;
        escalaOriginal = transform.localScale;
    }

    public override void onEntry(Ponto ponto)
    {
        this.ponto = ponto;

        spriteRenderer.sprite = spritePonto;
        transform.position = new Vector3(ponto.x, ponto.y, 0);
        Project.ScaleObject(
            spriteRenderer,
            transform,
            new Vector3(80f, 80f, 1f),
            escalaOriginal
        );

        texto.text = ponto.ordem.ToString();

        estadoDefinido = false; // o pool reaproveita o objeto: força a pintura
        AplicarEstado(EstadoCheckPoint.Fora);
    }

    public override void onUpdate(Ponto data)
    {
        ponto = data;
    }

    public override void onExit(Action complete)
    {
        complete?.Invoke();
    }

    /// <summary>Pinta o marcador conforme o progresso do frame atual (fase.progresso[indice]).</summary>
    public void MostrarEstado(Progresso progresso)
    {
        if (ponto == null) return;
        AplicarEstado(ponto.EstadoNo(progresso));
    }

    private void AplicarEstado(EstadoCheckPoint novo)
    {
        if (estadoDefinido && novo == estado) return;
        estado = novo;
        estadoDefinido = true;

        bool consumido = novo == EstadoCheckPoint.Consumido;
        if (spriteRenderer)
        {
            var sprite = consumido ? spriteConsumido : spritePonto;
            if (sprite) spriteRenderer.sprite = sprite;
            spriteRenderer.color = CorDoEstado(novo);
        }
        if (texto)
        {
            var cor = consumido ? corDentro : CorDoEstado(novo);
            if (consumido) cor.a *= alfaTextoConsumido;
            texto.color = cor;
        }
    }

    private Color CorDoEstado(EstadoCheckPoint e)
    {
        switch (e)
        {
            case EstadoCheckPoint.Dentro: return corDentro;
            case EstadoCheckPoint.Consumido: return corConsumido;
            default: return corFora;
        }
    }
}}