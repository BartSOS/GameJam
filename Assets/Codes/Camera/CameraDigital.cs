using System.Threading.Tasks;
using UnityEngine;
using Datas;
using Cysharp.Threading.Tasks;

namespace WebCam
{

public class CameraDigital : ADependsProject
{
    public Camera cam;
    private float altura;
    private float largura;
    private float width_engine;
    private float height_engine;

    public float Altura => altura;
    public float Largura => largura;
    public float Largura_Engine => soDepoisDeCarregarProject(width_engine);
    public float Altura_Engine => soDepoisDeCarregarProject(height_engine);

    public float Altura_Mundo => altura;
    public float Largura_Mundo => largura;

    public Vector2 Tam_Mundo => new Vector3(altura, altura);
    public Vector2 Tam_Engine => soDepoisDeCarregarProject(new Vector3(width_engine, height_engine));
    public Vector2 Tam_Tela => new Vector3(largura, altura);
    public Vector2 Posicao => transform.position;

    public Vector3 calculaTamanho(float ortoghraphicSize, float aspect)
    {
        float altura = ortoghraphicSize * 2f;
        float largura = altura * aspect;
        return new Vector3(largura, altura);
    }

    protected override void AoAcordar()
    {
        cam = Camera.main;
        altura = cam.orthographicSize * 2f;
        largura = altura * cam.aspect;
    }

    protected override UniTask AoCarregarProject(Project project)
    {
        width_engine = project.config.arena_width;
        height_engine = project.config.arena_height;
        return UniTask.CompletedTask;
    }
}   
}