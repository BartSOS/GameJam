using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void onEntry(float raio, float x, float y)
    {
        transform.position = new Vector3(x, y, 0);
        Project.ScaleObject(
            spriteRenderer,
            transform,
            new Vector3(raio * 2, raio * 2, 1f)
        );
    }
}