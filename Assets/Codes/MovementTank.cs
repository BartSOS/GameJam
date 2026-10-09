using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class MovementTank : MonoBehaviour
{
    private bool primeiraVez;
    private Vector2 posicao;
    private float hp;
    private bool shield;
    private Quaternion angle;
    private GameObject topo;
    public GameObject EnergyShield;
    private GameObject bas;
    private SpriteRenderer spriteRendererBase;
    private SpriteRenderer spriteRendererTopo;
    private Animator animatorShield;
    private Coroutine coroutine;
    private Coroutine coroutineShield; 
    private GameObject contornoFake;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        EnergyShield.SetActive(false);
        animatorShield = EnergyShield.GetComponent<Animator>();

        primeiraVez = true;

        SpriteRenderer [] renderes = GetComponentsInChildren<SpriteRenderer>();
        spriteRendererTopo = renderes[0];
        spriteRendererBase = renderes[1];

        scaleTank();
    }

    void Start()
    {
        contornoFake = new GameObject("ContornoFake");
        CriaChildren(transform.gameObject, contornoFake);
        contornoFake.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        GameObject childFakeTopo = new GameObject("ChildTopo");
        GameObject childFakeBase = new GameObject("ChildBase");

        CriaChildren(contornoFake, childFakeTopo);
        CriaChildren(contornoFake, childFakeBase);

        SpriteRenderer contornoFakeRendererBase = childFakeBase.AddComponent<SpriteRenderer>();
        SpriteRenderer contornoFakeRendererTopo = childFakeTopo.AddComponent<SpriteRenderer>();

        criaContorno(contornoFakeRendererBase, spriteRendererBase);
        criaContorno(contornoFakeRendererTopo, spriteRendererTopo);

        contornoFake.SetActive(false);
    }

    private void criaContorno(SpriteRenderer spriteRenderer, SpriteRenderer spriteOriginal)
    {
        spriteRenderer.sprite = spriteOriginal.sprite;
        spriteRenderer.material.shader = Shader.Find("GUI/Text Shader");
        spriteRenderer.color = "#00A8FF".FromHex(); // Cor azul claro
        spriteRenderer.sortingOrder = spriteOriginal.sortingOrder - 2;
    }

    private void CriaChildren(GameObject parent, GameObject child)
    {
        child.transform.SetParent(parent.transform, false);
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = Vector3.one;
        contornoFake.transform.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (primeiraVez) return;

        transform.position = Vector2.Lerp(
            transform.position,
            posicao,
            Time.deltaTime * 15f
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            angle, 
            Time.deltaTime * 15f);
    }

    private void scaleTank()
    {
        topo = spriteRendererTopo.gameObject;
        bas = spriteRendererBase.gameObject;

        Vector3 tamSpriteBase = spriteRendererBase.sprite.bounds.size;

        CameraDigital cam = Camera.main.GetComponent<CameraDigital>();

        Vector3 scaleMax = Project.regraDeTres(new Vector3(40f, 40f), cam.Tam_Engine, cam.Tam_Mundo);

        transform.localScale = Project.regraDeTres(transform.localScale, tamSpriteBase, scaleMax);
    }

    public void updateValues(float x, float y, float angle, float hp, bool shield)
    {
        posicao = new Vector2(x, y);
        this.angle = Quaternion.Euler(0f, 0f, angle - 90f);

        if (primeiraVez)
        {
            transform.position = posicao;
            transform.rotation = this.angle;
            primeiraVez = false;
        }
        else
        {
            if (hp < this.hp)
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);

                StartCoroutine(animaDano());
            }

        }

        this.hp = hp;

        if (coroutineShield != null && this.shield != shield)
            StopCoroutine(coroutineShield);
        
        this.shield = shield;

        if (shield)
           coroutineShield = StartCoroutine(animaShield());
        else
            if (EnergyShield.activeSelf)
                coroutineShield = StartCoroutine(exitShield());

    }

    private IEnumerator exitShield()
    {
        animatorShield.Play("ShieldQuebrando");
        animatorShield.Update(0f);

        float tempo = animatorShield.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(tempo);

        if (!shield && EnergyShield.activeSelf)
            EnergyShield.SetActive(false);
    }

    private IEnumerator animaShield()
    {
        EnergyShield.SetActive(true);
        yield return null;

        animatorShield.Play("ShieldAtivando");
        animatorShield.Update(0f);

        float tempo = animatorShield.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(tempo);

        if (shield && EnergyShield.activeSelf)
            animatorShield.Play("Shield");
    }

    private IEnumerator animaDano()
    {

        for (int i = 0; i < 4; i++)
        {
            spriteRendererBase.color = Color.red;
            spriteRendererTopo.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRendererBase.color = Color.white;
            spriteRendererTopo.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void BrilharTank()
    {
        contornoFake.SetActive(true);
        contornoFake.transform.DOKill();
    }

    public void PararBrilhoTank()
    {
        contornoFake.transform.DOKill();
        contornoFake.SetActive(false);
    }
}
