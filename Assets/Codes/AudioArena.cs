using UnityEngine;
using TMPro;
using Audio;
using NUnit.Framework.Constraints;

public class AudioArena : MonoBehaviour
{
    public AudioClip somEntry;
    public AudioClip somExit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSFX.INSTANCE.PlayLoop(somEntry);
        Debug.LogError("Ta tocando");
    }

    public void tocaSomSaida()
    {
        AudioSFX.INSTANCE.PlayLoop(somExit);
    }
}
