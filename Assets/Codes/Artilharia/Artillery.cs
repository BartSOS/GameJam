using System;
using UnityEngine;
using Datas;
using Pooling;
using Datas.Frames;
using Audio;

namespace Artilharia
{
public class Artillery : APoolAbleMonobehaviour<Hazard>
{
    public GameObject circle;
    public GameObject explosion;
    public Sprite lastSprite;

    [Header("Áudio")]
    [Tooltip("Toca (AudioSFX) quando a bomba explode.")]
    [SerializeField] private AudioClip somExplosao;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float timeTotal;
    private Action aoTerminar;

    void Awake()
    {
        animator = explosion.GetComponent<Animator>();
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        explosion.GetComponent<Explosao>().FimExplodindo += OnFimExplodindo;
    }

    public override void onEntry(Hazard hazard)
    {
        aoTerminar = null;
        explosion.SetActive(false);
        circle.SetActive(true);
        circle.transform.localScale = Vector3.zero;

        transform.position = new Vector2(hazard.x, hazard.y);

        timeTotal = Mathf.Max(hazard.timer, 0.0001f);

        spriteRenderer.sprite = lastSprite;
        Vector3 diametro = new Vector3(hazard.radius * 2f, hazard.radius * 2f, 1f);
        Project.ScaleObject(spriteRenderer, animator.transform, diametro, Vector3.one);
    }

    public override void onUpdate(Hazard hazard)
    {
        float progress = Mathf.Clamp01(1f - hazard.timer / timeTotal);
        circle.transform.localScale = Vector3.one * progress;
    }

    public override void onExit(Action onComplete)
    {
        aoTerminar = onComplete;
        circle.SetActive(false);
        explosion.SetActive(true);
        animator.Play("Explodindo", 0, 0f);
        if (somExplosao && AudioSFX.INSTANCE) AudioSFX.INSTANCE.Play(somExplosao);
    }

    private void OnFimExplodindo()
    {
        explosion.SetActive(false);
        var callback = aoTerminar;
        aoTerminar = null;
        callback?.Invoke();
    }
}
}