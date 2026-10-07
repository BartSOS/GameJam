using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip somEntry;
    public AudioClip somExit;
    public TextMeshProUGUI texto;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = somEntry;
        audioSource.loop = true;
        audioSource.Play();
        texto.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tocaSomSaida(string nameVencedor)
    {
        audioSource.Stop();
        audioSource.clip = somExit;
        audioSource.Play();

        texto.text = nameVencedor + " venceu!";
        texto.gameObject.SetActive(true);
    }
}
