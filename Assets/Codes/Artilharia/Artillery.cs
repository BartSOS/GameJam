using System.Collections;
using UnityEngine;
using Datas;
using Pooling;
using Datas.Frames;
using System;

namespace Artilharia{
public class Artillery : APoolAbleMonobehaviour<Hazard>
{
    public GameObject circle;
    public GameObject explosion;
    public Sprite lastSprite;
    private Vector3 scaleMax;
    private Animator animator;
    private float timeTotal;
    private Action aoTerminar;

    void Awake()
    {
        animator = explosion.GetComponent<Animator>();
        Explosao explosao = explosion.GetComponent<Explosao>();
        explosao.FimExplodindo += OnFimExplodindo;
    }

    public override void onEntry(Hazard hazard)
    {
        explosion.SetActive(false);

        transform.position = new Vector2(hazard.x, hazard.y);

        scaleMax = new Vector3(hazard.radius * 2, hazard.radius * 2, 1f);
        circle.transform.localScale = Vector3.zero;

        this.timeTotal = hazard.timer;
        explosion.transform.localScale = Project.regraDeTresMaster(explosion.transform.localScale, lastSprite.bounds.size, scaleMax);

    }

    public override void onUpdate(Hazard hazard)
    {   
        float progress = 1 - (hazard.timer / timeTotal);

        circle.transform.localScale = Vector3.Lerp(Vector3.zero, scaleMax, progress);
    }
    public override void onExit(Action onComplete)
    {
        aoTerminar = onComplete;
        explosion.SetActive(true);
        animator.Play("Explodindo");
    }

    public void OnFimExplodindo()
    {
        var callback = aoTerminar;
        aoTerminar = null;
        callback?.Invoke();
    }
}
}