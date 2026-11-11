using UnityEngine;

namespace Tanques
{
public class Shield : MonoBehaviour
{
    private Animator animatorShield;
    private bool _shield;
    public bool shield {
        set
        {
            if (_shield != value)
            {
                _shield = value;
                if (!_shield)
                    exitShield();

                else
                    animaShield();
            }
        }
        get => _shield;
    }

    void Awake()
    {
        animatorShield = GetComponent<Animator>();
        if (!_shield)
            gameObject.SetActive(false);
    }

    private void exitShield()
    {
        if (gameObject.activeSelf)
            animatorShield.Play("ShieldQuebrando", 0, 0f);
    }

    private void animaShield()
    {
        gameObject.SetActive(true);

        animatorShield.Play("ShieldAtivando", 0, 0f);
    }

    public void aoTerminarAtivando()
    {
        if (_shield && gameObject.activeSelf)
            animatorShield.Play("Shield");   
    }

    public void aoTerminarQuebrando()
    {
        if (!_shield && gameObject.activeSelf)
            gameObject.SetActive(false);   
    }
}
    
}