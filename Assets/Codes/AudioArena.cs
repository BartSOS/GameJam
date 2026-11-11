using UnityEngine;
using TMPro;
using Audio;

public class AudioArena : MonoBehaviour
{
    public AudioClip somEntry;
    public AudioClip somExit;
    public TextMeshProUGUI texto;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSFX.INSTANCE.PlayLoop(somEntry);
        texto.gameObject.SetActive(false);
    }

    public void tocaSomSaida(string nameVencedor)
    {
        AudioSFX.INSTANCE.PlayLoop(somExit);

        texto.text = nameVencedor + " venceu!";
        texto.gameObject.SetActive(true);
    }
}
