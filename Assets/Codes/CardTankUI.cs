using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Tanques;
using DG.Tweening;
using UnityEngine.InputSystem;

public class CardTankUI : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI texto;
    private Image barraHp;
    private Image barraShield;
    private float larguraMaxHp;
    private float larguraMaxEnergy;
    private float energyAntiga;
    private float hpAntiga;
    private bool selecionado = false;
    private int idTank;
    private float tempoAnimacao;
    private ArenaVisualizer arenaManager;

    void Awake()
    { 
        texto = GetComponentInChildren<TextMeshProUGUI>();

        Image [] images = GetComponentsInChildren<Image>();

        foreach (Image img in images)
        {
            if (img.gameObject.name == "BarraHp")
                barraHp = img;
            else if (img.gameObject.name == "BarraShield")
                barraShield = img;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tempoAnimacao = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        barraHp.rectTransform.sizeDelta = Vector2.Lerp(barraHp.rectTransform.sizeDelta, new Vector2(larguraMaxHp * (hpAntiga / 100f), barraHp.rectTransform.sizeDelta.y), 10f * Time.deltaTime);
        barraShield.rectTransform.sizeDelta = Vector2.Lerp(barraShield.rectTransform.sizeDelta, new Vector2(larguraMaxEnergy * (energyAntiga / 100f), barraShield.rectTransform.sizeDelta.y), 10f * Time.deltaTime);
    
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Mouse.current.position.ReadValue(), null))
            {
                if (selecionado)
                    DesmarcaCard();
                selecionado = false;   
            }
        }
    }

    public void onEntry(int id, string name, ArenaVisualizer arena)
    {
        idTank = id;
        energyAntiga = 100f;
        larguraMaxHp = barraHp.rectTransform.sizeDelta.x;
        larguraMaxEnergy = barraShield.rectTransform.sizeDelta.x;
        arenaManager = arena;   
        texto.text = name;
        barraHp.fillAmount = 1f;
        barraShield.fillAmount = 1f;
        barraHp.color = "#FF0000".FromHex(); //color em hexadecimal??
        barraShield.color = "#00B2FF".FromHex(); //color em hexadecimal??
    }

    public void updateValues(float hp, float energy, bool shieldActive)
    {
        if (energy < energyAntiga)
            barraShield.color = "#73D1FA".FromHex(); //color em hexadecimal??

        else
            barraShield.color = "#00B2FF".FromHex(); //color em hexadecimal??
        
        energyAntiga = energy;
        hpAntiga = hp;
    }   

    public void OnPointerClick(PointerEventData eventData)
    {
        selecionado = !selecionado;

        transform.DOKill();

        if (selecionado)
        {
            transform.DOScale(1.1f, tempoAnimacao).SetEase(Ease.OutCubic);
            RectTransform rect = transform.GetComponent<RectTransform>();
            rect.DOAnchorPosY(rect.anchoredPosition.y + 10f, tempoAnimacao).SetEase(Ease.OutCubic);
            BrilharContornoTank();
        }
        else
        {
            DesmarcaCard();
        }
    }

    private void DesmarcaCard()
    {
        transform.DOScale(new Vector3(1f, 1f, 1f), tempoAnimacao).SetEase(Ease.OutCubic);
        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.DOAnchorPosY(rect.anchoredPosition.y - 10f, tempoAnimacao).SetEase(Ease.OutCubic);
        PararBrilhoTank();
    }

    public void BrilharContornoTank()
    {
        Tanque tank;

        if (arenaManager != null)
        {
            tank = arenaManager.getTankById(idTank);
        }

        else return;

        if (tank != null)
        {
            tank.BrilharTank();
        }
    }

    public void PararBrilhoTank()
    {
        Tanque tank;

        if (arenaManager != null)
        {
            tank = arenaManager.getTankById(idTank);
        }

        else return;

        if (tank != null)
        {
            tank.PararBrilhoTank();
        }
    }
}

public static class UtilsColor
{
    public static Color FromHex(this string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        else
        {
            return Color.white; // Return a default color if parsing fails
        }
    }
}