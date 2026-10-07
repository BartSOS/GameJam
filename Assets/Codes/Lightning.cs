using UnityEngine;

public class Lightning : MonoBehaviour
{
    private Animator animator;
    private Animator animatorChildren;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animatorChildren = GetComponentInChildren<Animator>();
        transform.localScale = Project.regraDeTresMaster(transform.localScale, GetComponent<SpriteRenderer>().sprite.bounds.size, new Vector3(40f, 40f));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onEntry(float x, float y)
    {
        transform.position = new Vector3(x, y, 0);
        animator.Play("LightningIdle");
        animatorChildren.Play("LightningChildren");
    }   
}
