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
    public Transform canvas;

    void Awake()
    {
        barraHp = new Bar(imageHp); 
        barraEnergy = new Bar(imageEnergy);

        canvas.name = $"CanvasTank_{gameObject.name}";
        // texto.text = gameObject.name;
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
        barraHp.updateBar(hp);
        barraEnergy.updateBar(energy);
    }

    public void SetBars(float hp, float energy)
    {
        barraHp.setBar(hp);
        barraEnergy.setBar(energy);
    }
}
}