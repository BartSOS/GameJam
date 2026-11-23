using UnityEngine;
using Datas;
using Unity.VisualScripting;
namespace Tiros
{
public class Cratera : ADestroy
{
    [SerializeField] private Vector2 limites;
    protected override void AwakeChildren()
    {
        float tamanho = Random.Range(limites.x, limites.y);
        Project.ScaleObject(spriteRenderer, transform, new Vector3(tamanho, tamanho, 1f), Vector3.one);
    }
    
}
}