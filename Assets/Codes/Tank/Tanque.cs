using System.Collections;
using UnityEngine;
using Datas;
using Datas.Frames;
using Pooling;
using System;

namespace Tanques
{

public class Tanque : APoolAbleMonobehaviour<Tank>
{
    private float hp;
    private float turret;
    private Vector2 posicao;
    private Quaternion angle;

    public SpriteRenderer spriteRendererBase;
    public SpriteRenderer spriteRendererTopo;

    private Coroutine coroutine;
    private Canvas canvas;
    private ContornoFake contornoFakeController;
    private Shield shieldController;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {   
        SpriteRenderer [] renderes = GetComponentsInChildren<SpriteRenderer>();
        spriteRendererTopo = renderes[0];
        spriteRendererBase = renderes[1];
        canvas = GetComponent<Canvas>();
        contornoFakeController = GetComponent<ContornoFake>();
        shieldController = GetComponentInChildren<Shield>();

        scaleTank();
    }
    

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(
            transform.position,
            posicao,
            Time.deltaTime * 15f
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            angle, 
            Time.deltaTime * 15f);

        spriteRendererTopo.transform.localRotation = Quaternion.Slerp(
            spriteRendererTopo.transform.localRotation, 
            Quaternion.Euler(0f, 0f, turret), 
            Time.deltaTime * 15f);
    }

    private void scaleTank()
    {
        Project.ScaleObject(
            spriteRendererBase,
            spriteRendererBase.transform,
            new Vector3(80f, 80f, 1f)
        );
    }

    public override void onEntry(Tank tank)
    {
        transform.position = posicao = new Vector2(tank.x, tank.y);
        transform.rotation = this.angle = Quaternion.Euler(0f, 0f, tank.angle - 90f);
        this.hp = tank.hp;
        shieldController.shield = tank.shield;

        transform.position = posicao;
        transform.rotation = this.angle;

        canvas.SetBars(this.hp, tank.energy);
    }

    public override void onUpdate(Tank tank)
    {
        posicao = new Vector2(tank.x, tank.y);
        this.angle = Quaternion.Euler(0f, 0f, tank.angle - 90f);
        this.turret = tank.turret;

        if (tank.hp < this.hp)
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
}
}