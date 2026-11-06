using UnityEngine;
using TMPro;
using Datas;

public class CheckPoint : MonoBehaviour
{
    private TextMeshProUGUI texto;
    private float consumido_tick;

    SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        texto = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void onEntry(float raio, float x, float y, float consumido_tick, int ordem)
    {
        transform.position = new Vector3(x, y, 0);
        Project.ScaleObject(
            spriteRenderer,
            transform,
            new Vector3(raio * 2, raio * 2, 1f)
        );

        this.consumido_tick = consumido_tick;
        texto.text = ordem.ToString();
    }
}