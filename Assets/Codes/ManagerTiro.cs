using UnityEngine;
using System.Collections;

public class ManagerTiro : MonoBehaviour
{
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
    void Awake()
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

    public void setValues(float x, float y)
    {
        posicao = new Vector2(x, y);
    }

    public IEnumerator onEntry(float angle, float x, float y)
    {
        audioSource.PlayOneShot(somEntry);
        transform.position = new Vector3(x, y);
        posicao = transform.position;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        posicaoTravada = transform.position;

        gameObject.SetActive(true);
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

    public IEnumerator onExit()
    {
        audioSource.PlayOneShot(somExit);
        posicaoTravada = transform.position;

        gameObject.SetActive(true);
        tiroParado.SetActive(true);
        tiroAndando.SetActive(false);

        yield return null;

        animatorParado.Play("Explodindo");
        animatorParado.Update(0f);

        float tempo = animatorParado.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(tempo);

        gameObject.SetActive(false);
        tiroParado.SetActive(false);
        tiroAndando.SetActive(false);
    }

}
