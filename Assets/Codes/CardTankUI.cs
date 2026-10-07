using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardTankUI : MonoBehaviour
{
    private TextMeshProUGUI texto;
    private Image barraHp;
    private Image barraShield;
    private float larguraMaxHp;
    private float larguraMaxEnergy;
    private float energyAntiga;
    private float hpAntiga;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        barraHp.rectTransform.sizeDelta = Vector2.Lerp(barraHp.rectTransform.sizeDelta, new Vector2(larguraMaxHp * (hpAntiga / 100f), barraHp.rectTransform.sizeDelta.y), 10f * Time.deltaTime);
        barraShield.rectTransform.sizeDelta = Vector2.Lerp(barraShield.rectTransform.sizeDelta, new Vector2(larguraMaxEnergy * (energyAntiga / 100f), barraShield.rectTransform.sizeDelta.y), 10f * Time.deltaTime);
    }

    public void onEntry(string name)
    {
        energyAntiga = 100f;
        larguraMaxHp = barraHp.rectTransform.sizeDelta.x;
        larguraMaxEnergy = barraShield.rectTransform.sizeDelta.x;

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