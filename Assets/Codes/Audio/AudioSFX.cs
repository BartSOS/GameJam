using UnityEngine;

namespace Audio
{
public class AudioSFX : MonoBehaviour
{
    private AudioSource audioSource;
    
    public static AudioSFX INSTANCE {get; private set;}

    void Awake()
    {
        INSTANCE = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip audioClip) => INSTANCE.audioSource.PlayOneShot(audioClip);
}    
}