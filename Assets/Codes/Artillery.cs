using System.Collections;
using UnityEngine;
using Datas;
using Pooling;

public class Artillery : MonoBehaviour
{
    public GameObject circle;
    public GameObject explosion;
    public Sprite lastSprite;
    private Animator animator;
    private float timeTotal;
    private Vector3 scaleMax;
    private Vector3 tamLastSprite;

    void Awake()
    {
        tamLastSprite = lastSprite.bounds.size;
        animator = explosion.GetComponent<Animator>();
    }

    public void onEntry(float radius, float x, float y, float timeTotal, float tamSh, float tamMundo)
    {
        explosion.SetActive(false);

        transform.position = new Vector2(x, y);

        float diametro = Project.regraDeTres(radius, tamSh, tamMundo) * 2f;

        scaleMax = new Vector3(diametro, diametro, 1f);
        circle.transform.localScale = Vector3.zero;

        this.timeTotal = timeTotal;
        explosion.transform.localScale = Project.regraDeTres(transform.localScale, tamLastSprite, scaleMax);

    }

    public void updateValues(float time)
    {   
        float progress = 1 - (time / timeTotal);

        circle.transform.localScale = Vector3.Lerp(Vector3.zero, scaleMax, progress);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator onExit()
    {
        explosion.SetActive(true); 
        yield return null;
        animator.Play("Explodindo");
        animator.Update(0f);

        float tempo = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(tempo);

        gameObject.SetActive(false);
    }
}
