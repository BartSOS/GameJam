using UnityEngine;
using Datas;

public class MedKit : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {   
        Vector3 tamLastSprite = GetComponent<SpriteRenderer>().sprite.bounds.size;
        transform.localScale = Project.regraDeTresMaster(transform.localScale, tamLastSprite, new Vector3(40f, 40f));

        animator = GetComponent<Animator>();
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
        animator.Play("MedKitIdle");
    }
}
