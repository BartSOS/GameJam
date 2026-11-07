using UnityEngine;
using System.Collections;
using Datas.Frames;
using Pooling;
using System;
public class ManagerTiro : AMonoBehaviourPool<ManagerTiro, Projectile>, IPoolable, IState<Projectile>
{
    public int Id{get; set;}
    public int Tick {get; set;}
    private Vector2 posicao;
    private Vector2 posicaoTravada;
    public GameObject tiroAndando;
    public GameObject tiroParado;
    private Animator animatorAndando;
    private Animator animatorParado;
    private AudioSource audioSource;

    public AudioClip somEntry;
    public AudioClip somExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        animatorAndando = tiroAndando.GetComponent<Animator>();
        animatorParado = tiroParado.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();   
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(
            transform.position,
            posicao,
            Time.deltaTime * 15f
        );

        if (tiroParado.activeSelf)
            tiroParado.transform.position = posicaoTravada;
    }

    public void onEntry(Projectile projectile)
    {
        float angle = Mathf.Atan2(projectile.vy, projectile.vx) * Mathf.Rad2Deg;
        posicao = transform.position = new Vector3(projectile.x, projectile.y);
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        posicaoTravada = transform.position;

        StartCoroutine(startAnimacao());
    }

    private IEnumerator startAnimacao()
    {
        audioSource.PlayOneShot(somEntry);

        tiroAndando.SetActive(true);
        tiroParado.SetActive(true);

        yield return null;

        animatorParado.Play("Atirando");
        animatorAndando.Play("TiroAndando");

        animatorParado.Update(0f);  
        animatorAndando.Update(0f);
        
        float tempo = animatorParado.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(tempo);

        if (tiroAndando.activeSelf)
            tiroParado.SetActive(false);
    }

    public void onUpdate(Projectile projectile)
    {
        posicao = new Vector2(projectile.x, projectile.y);
    }

    private IEnumerator endAnimacao(Action onComplete)
    {
        audioSource.PlayOneShot(somExit);
        posicaoTravada = transform.position;

        tiroParado.SetActive(true);
        tiroAndando.SetActive(false);

        yield return null;

        animatorParado.Play("Explodindo");
        animatorParado.Update(0f);

        float tempo = animatorParado.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(tempo);

        tiroParado.SetActive(false);
        tiroAndando.SetActive(false);

        onComplete.Invoke();        
    }

    public void onExit(Action onComplete)
    {
        StartCoroutine(endAnimacao(onComplete));
    }

}
