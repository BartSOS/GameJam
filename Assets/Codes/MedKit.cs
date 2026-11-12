using UnityEngine;
using Datas.Frames;
using Datas;
using Pooling;
using System;

public class MedKit : APoolAbleMonobehaviour<Item>
{
    private Animator animator;

    void Awake()
    {   

        animator = GetComponent<Animator>();
    }

    public override void onEntry(Item item)
    {
        Vector3 tamLastSprite = GetComponent<SpriteRenderer>().sprite.bounds.size;
        transform.localScale = Project.regraDeTresMaster(transform.localScale, tamLastSprite, new Vector3(40f, 40f));
        transform.position = new Vector3(item.x, item.y, 0);
        animator.Play("MedKitIdle");
    }

    public override void onExit(Action complete)
    {
        complete?.Invoke();   
    }

    public override void onUpdate(Item data)
    {
        
    }
}
