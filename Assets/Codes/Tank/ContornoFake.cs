using UnityEngine;

namespace Tanques
{
public class ContornoFake : MonoBehaviour
{
    private GameObject contornoFake;
    [SerializeField] private float scaleFactorContorno = 1.1f;
    void Start()
    {
        contornoFake = new GameObject("ContornoFake");
        CriaChildren(transform.gameObject, contornoFake);
        contornoFake.transform.localScale = new Vector3(scaleFactorContorno, scaleFactorContorno, scaleFactorContorno);
        GameObject childFakeTopo = new GameObject("ChildTopo");
        GameObject childFakeBase = new GameObject("ChildBase");

        CriaChildren(contornoFake, childFakeTopo);
        CriaChildren(contornoFake, childFakeBase);

        SpriteRenderer contornoFakeRendererBase = childFakeBase.AddComponent<SpriteRenderer>();
        SpriteRenderer contornoFakeRendererTopo = childFakeTopo.AddComponent<SpriteRenderer>();

        Tanque tanque = GetComponent<Tanque>();

        criaContorno(contornoFakeRendererBase, tanque.spriteRendererBase);
        criaContorno(contornoFakeRendererTopo, tanque.spriteRendererTopo);

        contornoFake.SetActive(false);   
    }

    private void criaContorno(SpriteRenderer spriteRenderer, SpriteRenderer spriteOriginal)
    {
        spriteRenderer.sprite = spriteOriginal.sprite;
        spriteRenderer.material.shader = Shader.Find("GUI/Text Shader");
        spriteRenderer.color = "#00A8FF".FromHex(); // Cor azul claro
        spriteRenderer.sortingOrder = spriteOriginal.sortingOrder - 2;
    }

    private void CriaChildren(GameObject parent, GameObject child)
    {
        child.transform.SetParent(parent.transform, false);
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = Vector3.one;
        contornoFake.transform.localRotation = Quaternion.identity;
    }    

    public void Active()
    {
        contornoFake.SetActive(true);
    }

    public void Deactive()
    {
        contornoFake.SetActive(false);
    }
}
}