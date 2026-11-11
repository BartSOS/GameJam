using System.Collections;
using Pooling;
using Datas.Frames;
using UnityEngine;
using WebCam;
using System.Runtime.InteropServices;
using Datas;
using System;

public class Laser : APoolAbleMonobehaviour<Hazard>
{
    private SpriteRenderer spriteRenderer;
    private Animator animatorLaser;
    private GameObject children;
    private CameraDigital cameraDigital;
    private float timer;
    private Quaternion angle;
    private bool atirando;
    private Action aoTerminar;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animatorLaser = GetComponentInChildren<Animator>();
        children = spriteRenderer.gameObject;
        cameraDigital = Camera.main.GetComponent<CameraDigital>();
    }
    public override void onUpdate(Hazard hazard)
    {
        this.angle = Quaternion.Euler(0, 0, hazard.angle);
        this.timer = hazard.timer;
    }

    public override void onEntry(Hazard hazard)
    {
        atirando = false;

        Sprite spriteAtual = spriteRenderer.sprite;
        spriteRenderer.transform.localScale = Project.regraDeTresMaster(spriteRenderer.transform.localPosition, spriteAtual.bounds.size, new Vector3(50f, 50f, 1f));
        float larguraCabecaPixels = spriteAtual.border.x;
        float larguraCabecaMundo = larguraCabecaPixels / spriteAtual.pixelsPerUnit;

        children.transform.localPosition = new Vector2(-(larguraCabecaMundo / 2), 0);

        spriteRenderer.size = new Vector2(cameraDigital.Largura * 1.7f, spriteRenderer.size.y);
        transform.rotation = Quaternion.Euler(0, 0, hazard.angle);


        animatorLaser.Play("LaserSurgindo");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraDigital.Posicao;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            angle,
            Time.deltaTime * 15f
        );

        if (timer < 0.1f && !atirando)
        {
            atirando = true;
            animatorLaser.Play("Laser");
        }
    }

    public override void onExit(Action onComplete)
    {
        animatorLaser.Play("LaserFechando");
    }

    public void AoFimFechando()
    {
        var callback = aoTerminar;
        aoTerminar = null;
        callback?.Invoke();
    }
}
