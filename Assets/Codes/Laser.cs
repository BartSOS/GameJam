using System.Collections;
using Pooling;
using Datas.Frames;
using UnityEngine;
using WebCam;

public class Laser : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animatorLaser;
    private GameObject children;
    private CameraDigital cameraDigital;
    private float timer;
    private Quaternion angle;
    private bool atirando;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animatorLaser = GetComponentInChildren<Animator>();
        children = spriteRenderer.gameObject;
        cameraDigital = Camera.main.GetComponent<CameraDigital>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void updateValues(float angle, float duration, float time)
    {
        this.angle = Quaternion.Euler(0, 0, angle);
        this.timer = time;
    }

    public void onEntry(float angle)
    {
        atirando = false;
        Sprite spriteAtual = spriteRenderer.sprite;

        float larguraCabecaPixels = spriteAtual.border.x;

        float larguraCabecaMundo = larguraCabecaPixels / spriteAtual.pixelsPerUnit;

        children.transform.localPosition = new Vector2(-(larguraCabecaMundo / 2), 0);

        spriteRenderer.size = new Vector2(cameraDigital.Largura * 1.7f, spriteRenderer.size.y);

        transform.rotation = Quaternion.Euler(0, 0, angle);

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

    public IEnumerator onExit()
    {
        animatorLaser.Play("LaserFechando");

        yield return null;

        float tempo = animatorLaser.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(tempo);

        gameObject.SetActive(false);
    }
}
