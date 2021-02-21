using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MovementSounds : MonoBehaviour
{
    public List<AudioClip> leftFootstep, rightFootstep, ghostMove, demonJump;

    public AudioClip playerJump, demonLand;

    private AudioSource _audio;
    private int _counter = 0;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void LeftStep()
    {
        _audio.PlayOneShot(leftFootstep[(_counter++ % 2)]);
    }

    public void RightStep()
    {
        _audio.PlayOneShot(rightFootstep[(_counter++ % 2)]);
    }
    
    public void PlayerJump()
    {
        _audio.PlayOneShot(playerJump);
    }

    public void GhostStep()
    {
        _audio.PlayOneShot(ghostMove[(_counter++ % 2)]);
    }

    public void DemonJump()
    {
        _audio.PlayOneShot(demonJump[(_counter++ % 2)]);
    }

    public void DemonLand()
    {
        _audio.PlayOneShot(demonLand);
    }
}
