using UnityEngine;

/// <summary>
/// Deixa marcas de esteira atrás do tanque, carimbando por distância percorrida.
/// Coloque no objeto do tanque que se move. O prefab da marca deve ter o script de auto destruição.
/// O sprite da marca deve estar deitado no eixo X (frente apontando para +X) e com pivô Right.
/// </summary>
[DefaultExecutionOrder(100)] // roda depois do script que faz o Lerp da posição
public class RastroSpawn : MonoBehaviour
{
    // ─────────────────────────────── Inspector ───────────────────────────────

    [Header("Marca")]
    [Tooltip("Prefab da marca da esteira (com o script de auto destruição).")]
    public GameObject prefabMarca;

    [Tooltip("Comprimento de cada marca no mundo. 0 = usa o comprimento do sprite do tanque.")]
    public float comprimentoMarca = 0f;

    [Tooltip("Largura da marca no mundo (de fora a fora das duas esteiras). 0 = proporcional ao comprimento.")]
    public float larguraMarca = 0f;

    [Tooltip("1 = marcas encostadas; menor que 1 = sobrepõem um pouco.")]
    [Range(0.5f, 1.2f)] public float fatorEspacamento = 1f;

    [Tooltip("Quanto recuar a marca atrás do centro do tanque (unidades do mundo).")]
    public float recuo = 0.1f;

    [Header("Rotação")]
    [Tooltip("Ligado: usa a rotação do tanque (bom para andar de ré). Desligado: usa a direção do movimento.")]
    public bool usarRotacaoDoTanque = false;

    [Tooltip("Correção de ângulo caso o sprite não esteja deitado no eixo X (ex.: 90 ou -90).")]
    public float correcaoAngulo = 0f;

    [Header("Segurança")]
    [Tooltip("Saltos maiores que isso num único frame não desenham rastro (respawn/teleporte).")]
    public float saltoMaximo = 3f;

    [Header("Debug")]
    public bool debugLogs = false;

    // ─────────────────────────────── Estado ───────────────────────────────

    const string TagPai = "Marcas";

    float comprimento;       // comprimento final da marca no mundo
    float espacamentoReal;   // distância percorrida entre uma marca e outra
    Vector3 escalaMarca;     // escala aplicada em cada marca criada
    Transform paiDasMarcas;  // objeto da cena com a tag "Marcas" (opcional)

    Vector3 ultimaMarca;     // ponto onde a última marca foi carimbada
    Vector3 posAnterior;     // posição do tanque no frame anterior
    bool temUltima;

    // ─────────────────────────────── Ciclo de vida ───────────────────────────────

    void Awake()
    {
        if (!CalcularTamanho())
        {
            enabled = false;
            return;
        }

        var pai = GameObject.FindWithTag(TagPai);
        paiDasMarcas = pai ? pai.transform : null;
        if (paiDasMarcas == null)
            Debug.LogWarning($"[Rastro:{name}] Nenhum objeto com a tag '{TagPai}'; as marcas vão para a raiz da cena.", this);
    }

    void OnEnable() => Reiniciar();

    /// Chame depois de posicionar o tanque no spawn/respawn (ex.: no onEntry do pool).
    public void Reiniciar() => temUltima = false;

    void LateUpdate()
    {
        Vector3 atual = transform.position;

        // primeiro frame depois de ativar/reiniciar: só marca o ponto de partida
        if (!temUltima)
        {
            ultimaMarca = posAnterior = atual;
            temUltima = true;
            return;
        }

        // teleporte = salto grande NESTE frame (não a distância acumulada)
        if ((atual - posAnterior).magnitude > saltoMaximo)
        {
            ultimaMarca = posAnterior = atual;
            return;
        }
        posAnterior = atual;

        Vector3 delta = atual - ultimaMarca;
        delta.z = 0f;
        float dist = delta.magnitude;
        if (dist < espacamentoReal) return; // ainda não andou o bastante

        Vector3 dir = delta / dist;
        Quaternion rot = CalcularRotacao(dir);

        // preenche o caminho todo, uma marca a cada espacamentoReal
        while (dist >= espacamentoReal)
        {
            ultimaMarca += dir * espacamentoReal;

            // pivô Right: a borda da frente fica no ponto, a marca cresce para trás
            Vector3 pos = ultimaMarca - dir * recuo;
            pos.z = transform.position.z;

            CriarMarca(pos, rot);
            dist -= espacamentoReal;
        }
    }

    // ─────────────────────────────── Auxiliares ───────────────────────────────

    /// Calcula comprimento, escala e espaçamento a partir do sprite da marca.
    bool CalcularTamanho()
    {
        if (prefabMarca == null)
        {
            Debug.LogError($"[Rastro:{name}] prefabMarca está vazio.", this);
            return false;
        }

        var sr = prefabMarca.GetComponentInChildren<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
        {
            Debug.LogError($"[Rastro:{name}] O prefab da marca não tem SpriteRenderer com sprite.", this);
            return false;
        }

        Vector2 tamanhoSprite = sr.sprite.bounds.size;
        Vector3 escalaBase = prefabMarca.transform.localScale;

        // comprimento desejado: o do Inspector ou, se for 0, o do sprite do tanque
        comprimento = comprimentoMarca;
        if (comprimento <= 0f)
        {
            var srTanque = GetComponentInChildren<SpriteRenderer>();
            comprimento = srTanque && srTanque.sprite
                ? srTanque.sprite.bounds.size.x * Mathf.Abs(srTanque.transform.lossyScale.x)
                : 0.5f;
        }

        float fatorX = comprimento / Mathf.Max(tamanhoSprite.x * Mathf.Abs(escalaBase.x), 0.0001f);
        float fatorY = larguraMarca > 0f
            ? larguraMarca / Mathf.Max(tamanhoSprite.y * Mathf.Abs(escalaBase.y), 0.0001f)
            : fatorX; // sem largura definida, mantém a proporção

        escalaMarca = new Vector3(escalaBase.x * fatorX, escalaBase.y * fatorY, escalaBase.z);
        espacamentoReal = Mathf.Max(comprimento * fatorEspacamento, 0.01f);

        if (debugLogs)
            Debug.Log($"[Rastro:{name}] comprimento={comprimento:F2} espacamento={espacamentoReal:F2} escala={escalaMarca}", this);

        return true;
    }

    Quaternion CalcularRotacao(Vector3 dir)
    {
        if (usarRotacaoDoTanque)
            return transform.rotation * Quaternion.Euler(0, 0, correcaoAngulo);

        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + correcaoAngulo;
        return Quaternion.Euler(0, 0, angulo);
    }

    void CriarMarca(Vector3 pos, Quaternion rot)
    {
        var marca = Instantiate(prefabMarca, pos, rot, paiDasMarcas);
        marca.transform.localScale = escalaMarca;

        if (debugLogs) Debug.Log($"[Rastro:{name}] marca criada em {pos}", marca);
    }
}