using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuAudioController : MonoBehaviour
{
    public AudioClip whooshLeft, whooshRight;

    private AudioSource _audio;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void Scroll(bool isLeft)
    {
        if (_audio)
            _audio.PlayOneShot(isLeft ? whooshLeft : whooshRight);
    }
}
