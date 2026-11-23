using UnityEngine;
using Datas;
namespace Tanques
{
public class Rastro : ADestroy
{
    protected override void AwakeChildren()
    {
        Project.ScaleObject(spriteRenderer, transform, new Vector3(20f, 20f, 1f), Vector3.one);
    }
}
}