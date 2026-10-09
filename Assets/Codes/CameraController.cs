using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraDigital cameraDigital;
    private float padding;
    private float tempoAnimacao;
    private Vector2 posicaoAlvo;
    private float reducao;
    [Header("Interface (UI)")]
    public RectTransform painelStatus;
    [SerializeField] private float posicaoOculta = -500f;
    [SerializeField] private float posicaoVisivel = 0f;

    void Awake()
    {
        reducao = 0.5f;
        tempoAnimacao = 0.5f;
        padding = 0.05f;
        cameraDigital = GetComponent<CameraDigital>();
    }

    void Start()
    {
        posicaoAlvo = new Vector2(padding, (1 - reducao) / 2);

        if (painelStatus != null)
        {
            float pixelWidth = ((RectTransform)painelStatus.parent).rect.width;
            float porcentagemSobra = 1 - 2*padding - reducao;
            float larguraPainel = pixelWidth * porcentagemSobra;
            painelStatus.anchoredPosition = new Vector2(posicaoOculta, painelStatus.anchoredPosition.y);
            painelStatus.sizeDelta = new Vector2(larguraPainel, painelStatus.sizeDelta.y);
        }

    }

    public void ZoomIn()
    {
        cameraDigital.cam.DOKill();
        transform.DOKill();

        if (painelStatus != null)
            painelStatus.DOKill();

        Rect rect = new Rect(posicaoAlvo, new Vector2(reducao, reducao));
        cameraDigital.cam.DORect(rect, tempoAnimacao).SetEase(Ease.OutCubic);

        if (painelStatus != null)
            painelStatus.DOAnchorPosX(posicaoVisivel, tempoAnimacao).SetEase(Ease.OutCubic);
    }

    public void ZoomOut()
    {
        cameraDigital.cam.DOKill();
        transform.DOKill();

        if (painelStatus != null)
            painelStatus.DOKill();

        cameraDigital.cam.DORect(new Rect(Vector2.zero, new Vector2(1f, 1f)), tempoAnimacao).SetEase(Ease.OutCubic);

        if (painelStatus != null)
            painelStatus.DOAnchorPosX(posicaoOculta, tempoAnimacao).SetEase(Ease.OutCubic);
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