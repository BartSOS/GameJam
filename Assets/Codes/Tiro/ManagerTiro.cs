using UnityEngine;
using Datas.Frames;
using Pooling;
using System;
using DG.Tweening;
using Audio;

namespace Tiros
{
public class ManagerTiro : APoolAbleMonobehaviour<Projectile>
{
    // O Animator do TiroParado está com Culling Mode = Always Animate.
    // Loop Time desligado nos clips “Atirando” e “Explodindo”.
    private Vector2 posicaoTravada;

    public GameObject tiroAndando;
    public GameObject tiroParado;
    
    private Animator animatorAndando;
    private Animator animatorParado;

    public AudioClip somEntry;
    public AudioClip somExit;

    private EventsObjectTiroParado eventsObjectTiroParado;
    private bool explodindo;
    private Action aoTerminar;

    private Tweener tween;
    private float ultimoUpdate;

    void Awake()
    {
        animatorAndando = tiroAndando.GetComponent<Animator>();
        animatorParado = tiroParado.GetComponent<Animator>();
        eventsObjectTiroParado = tiroParado.GetComponent<EventsObjectTiroParado>(); 
        eventsObjectTiroParado.FimAtirando += onFimAtirando;
        eventsObjectTiroParado.FimExplodindo += onFimExplodindo;
    }

    void LateUpdate()
    {
        if (tiroParado.activeSelf)
            tiroParado.transform.position = posicaoTravada;
    }


    public override void onEntry(Projectile projectile)
    {
        explodindo = false;
        float angle = Mathf.Atan2(projectile.vy, projectile.vx) * Mathf.Rad2Deg;
        posicaoTravada = transform.position = new Vector3(projectile.x, projectile.y);
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        ultimoUpdate = Time.time;

        startAnimacao();
    }

    private void startAnimacao()
    {
        // AudioSFX.INSTANCE.Play(somEntry);

        tiroAndando.SetActive(true);
        tiroParado.SetActive(true);

        animatorParado.Play("Atirando", 0, 0f);
        animatorAndando.Play("TiroAndando", 0, 0f);
    }

    public override void onUpdate(Projectile projectile)
    {
        float intervalo = Mathf.Max(Time.time - ultimoUpdate, Time.deltaTime);
        ultimoUpdate = Time.time;

        Vector3 pos = new Vector3(projectile.x, projectile.y);

        if (tween == null)
        {
            tween = transform.DOMove(pos, intervalo).SetEase(Ease.Linear).SetAutoKill(false).SetLink(gameObject);            
        }

        else
            tween.ChangeValues(transform.position, pos, intervalo).Restart();
    }

    private void endAnimacao()
    {
        // AudioSFX.INSTANCE.Play(somExit);

        tiroParado.SetActive(true);
        tiroAndando.SetActive(false);

        animatorParado.Play("Explodindo", 0, 0f);
    }

    public override void onExit(Action onComplete)
    {
        tween?.Complete();
        aoTerminar = onComplete;
        explodindo = true;
        posicaoTravada = transform.position;
        endAnimacao();
    }

    private void onFimAtirando()
    {
        if (!explodindo)
            tiroParado.SetActive(false);   
    }

    private void onFimExplodindo()
    {
        tiroParado.SetActive(false);
        tiroAndando.SetActive(false);

        var callback = aoTerminar;
        aoTerminar = null;

        callback?.Invoke();        
    }
}
}