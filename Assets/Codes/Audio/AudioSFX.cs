using UnityEngine;

namespace Audio
{
public class AudioSFX : MonoBehaviour
{
    [SerializeField]private AudioSource audioSourceShot;
    [SerializeField] private AudioSource audioSourceLoop;
    
    public static AudioSFX INSTANCE {get; private set;}

    void Awake()
    {
        INSTANCE = this;
        audioSourceLoop.loop = true;
    }

    public void Play(AudioClip audioClip) => INSTANCE.audioSourceShot.PlayOneShot(audioClip);
    public void PlayLoop(AudioClip audioClip)
    {
        INSTANCE.audioSourceLoop.Stop();
        INSTANCE.audioSourceLoop.clip = audioClip;
        INSTANCE.audioSourceLoop.Play();
    }
}    
}