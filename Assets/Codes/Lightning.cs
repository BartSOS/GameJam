using UnityEngine;
using Datas;
using Pooling;
using Datas.Frames;
using System;

public class Lightning : APoolAbleMonobehaviour<Item>
{
    private Animator animator;
    private Animator animatorChildren;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animatorChildren = GetComponentsInChildren<Animator>()[1];
        transform.localScale = Project.regraDeTresMaster(transform.localScale, GetComponent<SpriteRenderer>().sprite.bounds.size, new Vector3(40f, 40f));
    }

    public override void onEntry(Item item)
    {
        transform.position = new Vector3(item.x, item.y, 0);
        animator.Play("LightningIdle");
        animatorChildren.Play("LightningChildren");
    }   

    
    public override void onExit(Action complete)
    {
        complete?.Invoke();   
    }

    public override void onUpdate(Item data)
    {
        
    }
}
