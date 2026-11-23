using UnityEngine;

public abstract class ADestroy : MonoBehaviour
{
    protected ArenaVisualizer arena;
    protected SpriteRenderer spriteRenderer;

    [SerializeField] protected int delta = 15;
    [SerializeField] protected int duracaoFade = 10;

    int tickInicial = -1;
    float alfaOriginal = 1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        arena = GameObject.FindWithTag("GameController").GetComponent<ArenaVisualizer>();
        alfaOriginal = spriteRenderer.color.a;
        tickInicial = arena.Indice;
        AwakeChildren();
    }

    protected virtual void AwakeChildren() { }

    void Update()
    {
        int decorrido = arena.Indice - tickInicial;

        if (decorrido < 0)
        {
            tickInicial = arena.Indice;
            return;
        }

        if (decorrido <= delta)
        {
            DefinirAlfa(alfaOriginal);
            return;
        }

        float progresso = (decorrido - delta) / (float)Mathf.Max(duracaoFade, 1);

        if (progresso >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        DefinirAlfa(alfaOriginal * (1f - progresso));
    }

    void DefinirAlfa(float a)
    {
        var c = spriteRenderer.color;
        c.a = a;
        spriteRenderer.color = c;
    }
}