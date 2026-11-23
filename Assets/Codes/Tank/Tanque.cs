
using System.Collections;
using UnityEngine;
using Datas;
using Datas.Frames;
using Pooling;
using System;
using DG.Tweening;

namespace Tanques
{
 
public class Tanque : APoolAbleMonobehaviour<Tank>
{
    private float hp;
    private float turret;
    private Vector2 posicao;
    private Quaternion angle;
    private Quaternion rotTopo;
 
    [SerializeField] private Sprite [] spritesTopo;
    [SerializeField] private Sprite [] spritesBase;
 
    public SpriteRenderer spriteRendererBase;
    public SpriteRenderer spriteRendererTopo;
 
    private Coroutine coroutine;
    private Canvas canvas;
    private ContornoFake contornoFakeController;
    private Shield shieldController;
    private Vector3 escalaOriginal;
 
    void Awake()
    {
        SpriteRenderer [] renderes = GetComponentsInChildren<SpriteRenderer>();
        spriteRendererTopo = renderes[0];
        spriteRendererBase = renderes[1];
        canvas = GetComponent<Canvas>();
        contornoFakeController = GetComponent<ContornoFake>();
        shieldController = GetComponentInChildren<Shield>();
        gameObject.name = $"Tank_Generico_{Id}";
        escalaOriginal = transform.localScale;
    }
 
    void Update()
    {
        float t = 1f - Mathf.Exp(-15f * Time.deltaTime);
 
        transform.position = Vector2.Lerp(transform.position, posicao, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, angle, t);
 
        rotTopo = Quaternion.Slerp(rotTopo, Quaternion.Euler(0f, 0f, turret - 90f), t);
        spriteRendererTopo.transform.rotation = rotTopo;
    }
 
    private void scaleTank()
    {
        Project.ScaleObject(
            spriteRendererBase,
            spriteRendererBase.transform,
            new Vector3(80f, 80f, 1f),
            escalaOriginal
        );
 
        Project.ScaleObject(
            spriteRendererBase,
            spriteRendererTopo.transform,
            new Vector3(80f, 80f, 1f),
            escalaOriginal
        );
    }
 
    public override void onEntry(Tank tank)
    {
        gameObject.name = LoadJson.project.GetNomeTank(tank.id);
        scaleTank();
        configurarCores();
 
        transform.position = posicao = new Vector2(tank.x, tank.y);
        transform.rotation = this.angle = Quaternion.Euler(0f, 0f, tank.angle - 90f);
 
        this.turret = tank.turret;
        rotTopo = Quaternion.Euler(0f, 0f, turret - 90f);
        spriteRendererTopo.transform.rotation = rotTopo;
 
        this.hp = tank.hp;
        shieldController.shield = tank.shield;
 
        GetComponent<RastroSpawn>()?.Reiniciar();
        var trail = GetComponentInChildren<TrailRenderer>();
        if (trail) trail.Clear();
 
        canvas.SetBars(this.hp, tank.energy, LoadJson.project.corTemaTanque[tank.id]);
    }
 
    public override void onUpdate(Tank tank)
    {
        posicao = new Vector2(tank.x, tank.y);
        this.angle = Quaternion.Euler(0f, 0f, tank.angle - 90f);
        this.turret = tank.turret;
 
        if (tank.hp < this.hp && gameObject.activeInHierarchy)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
 
            coroutine = StartCoroutine(animaDano());
        }
 
        this.hp = tank.hp;
        shieldController.shield = tank.shield;
        canvas.UpdateBars(this.hp, tank.energy);
    }
 
    public override void onExit(Action complete)
    {
        complete?.Invoke();
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
        contornoFakeController.Active();
    }
 
    public void PararBrilhoTank()
    {
        contornoFakeController.Deactive();
    }
 
    public void configurarCores()
    {
        Sprite topo = spritesTopo[Id % spritesTopo.Length];
        spriteRendererTopo.sprite = topo;
 
        Sprite bases = spritesBase[Id % spritesBase.Length];
        spriteRendererBase.sprite = bases;
    }

    void OnDisable()
    {
        coroutine = null;
        spriteRendererBase.color = Color.white;
        spriteRendererTopo.color = Color.white;
    }
    }
}
 