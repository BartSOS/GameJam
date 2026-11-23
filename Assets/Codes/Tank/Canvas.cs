using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tanques
{
public class Canvas : MonoBehaviour
{
    private Bar barraHp;
    private Bar barraEnergy;   
    
    public Image imageHp;
    public Image imageEnergy;
    public TextMeshProUGUI texto;
    public TextMeshProUGUI hp;
    public Image rect;
    public Transform canvas;

    void Awake()
    {
        barraHp = new Bar(imageHp); 
        barraEnergy = new Bar(imageEnergy);

        canvas.name = $"CanvasTank_{gameObject.name}";
    }

    void LateUpdate()
    {
        canvas.rotation = Quaternion.identity;
        canvas.position = new Vector3(transform.position.x, transform.position.y, 0f);   
    }

    void OnEnable() => canvas.gameObject.SetActive(true);
    void OnDisable() => canvas.gameObject.SetActive(false);

    public void UpdateBars(float hp, float energy)
    {
        Debug.Log($"[{gameObject.name}] hp={hp} energy={energy}", this);
        barraHp.updateBar(hp);
        barraEnergy.updateBar(energy);
        this.hp.text = ((int)hp).ToString();

    }

    public void SetBars(float hp, float energy, Color corRect)
    {
        texto.text = gameObject.name;
        this.hp.text = ((int)hp).ToString();
        this.rect.color = corRect;
        barraHp.setBar(hp);
        barraEnergy.setBar(energy);
    }
}
}