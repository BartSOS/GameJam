using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraDigital cameraDigital;
    private float orthographicNormal;
    private float orthographicAlvo;
    private Vector3 posicaoAlvo;
    private Vector3 posicaoNormal;
    private float tempoAnimacao = 0.5f;

    void Awake()
    {
        cameraDigital = GetComponent<CameraDigital>();
        posicaoNormal = new Vector3(0, 0, -10);
    }

    void Start()
    {
        orthographicNormal = cameraDigital.cam.orthographicSize;
        orthographicAlvo = orthographicNormal*0.7f;
        Vector3 tamanhoNormal = cameraDigital.calculaTamanho(orthographicNormal, cameraDigital.cam.aspect);
        Vector3 tamanhoAlvo = cameraDigital.calculaTamanho(orthographicAlvo, cameraDigital.cam.aspect);

        posicaoAlvo = new Vector3(tamanhoAlvo.x/2 - tamanhoNormal.x/2, tamanhoAlvo.y/2 - tamanhoNormal.y/2, -10);
    }

    public void ZoomIn()
    {
        cameraDigital.cam.DOKill();
        transform.DOKill();

        cameraDigital.cam.DOOrthoSize(orthographicAlvo, tempoAnimacao).SetEase(Ease.OutCubic);
        cameraDigital.cam.transform.DOMove(posicaoAlvo, tempoAnimacao).SetEase(Ease.OutCubic);
    }

    public void ZoomOut()
    {
        cameraDigital.cam.DOKill();
        transform.DOKill();

        cameraDigital.cam.DOOrthoSize(orthographicNormal, tempoAnimacao).SetEase(Ease.OutCubic);
        cameraDigital.cam.transform.DOMove(posicaoNormal, tempoAnimacao).SetEase(Ease.OutCubic);
    }

    void Update()
    {
        // Verifica se existe um teclado conectado para não dar erro
        if (Keyboard.current != null) 
        {
            // wasPressedThisFrame é o equivalente ao GetKeyDown no novo sistema
            if (Keyboard.current.upArrowKey.wasPressedThisFrame) ZoomIn();
            if (Keyboard.current.downArrowKey.wasPressedThisFrame) ZoomOut();
        }
    }

}