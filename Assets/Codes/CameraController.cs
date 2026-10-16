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
    public RectTransform button;
    private float posicaoOculta;
    [SerializeField] private float posicaoVisivel = 0f;
    [SerializeField] private float offsetButton;
    private bool painelAberto = false;

    void Awake()
    {
        reducao = 0.6f;
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
            posicaoOculta = larguraPainel;
            painelStatus.anchoredPosition = new Vector2(posicaoOculta, 0f);
            painelStatus.sizeDelta = new Vector2(larguraPainel, ((RectTransform)painelStatus.parent).rect.height);
            button.anchoredPosition = new Vector2(offsetButton, button.anchoredPosition.y);
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
    }

    public void AlternarPainel()
    {
        painelAberto = !painelAberto;
        if (painelAberto)
        {
            ZoomIn();
        }
        else
        {
            ZoomOut();
        }
    }

}