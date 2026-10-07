using UnityEngine;


public class CameraDigital : MonoBehaviour
{
    public Camera cam;
    private float altura;
    private float largura;
    private float width_engine;
    private float height_engine;

    public float Altura => altura;
    public float Largura => largura;
    public float Largura_Engine => width_engine;
    public float Altura_Engine => height_engine;

    public Vector2 Tam_Mundo => new Vector3(altura, altura);
    public Vector2 Tam_Engine => new Vector3(width_engine, height_engine);
    public Vector2 Tam_Tela => new Vector3(largura, altura);
    public Vector2 Posicao => transform.position;

    public Vector3 calculaTamanho(float ortoghraphicSize, float aspect)
    {
        float altura = ortoghraphicSize * 2f;
        float largura = altura * aspect;
        return new Vector3(largura, altura);
    }

    void Awake()
    {
        cam = Camera.main;
        altura = cam.orthographicSize * 2f;
        largura = altura * cam.aspect;
    }

    public void configCamera(float width_engine, float height_engine)
    {
        this.width_engine = width_engine;
        this.height_engine = height_engine;
    }
    
}