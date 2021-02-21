using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MovementSounds : MonoBehaviour
{
    public List<AudioClip> leftFootstep, rightFootstep, ghostMove, demonJump;

    public AudioClip playerJump, demonLand, demonCaster;

    private AudioSource _audio;
    private int _counter = 0;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void LeftStep()
    {
        _audio.PlayOneShot(leftFootstep[(_counter++ % 2)], 0.25f);
    }

    public void RightStep()
    {
        _audio.PlayOneShot(rightFootstep[(_counter++ % 2)], 0.25f);
    }
    
    public void PlayerJump()
    {
        _audio.PlayOneShot(playerJump, 0.3f);
    }

    public void GhostStep()
    {
        _audio.PlayOneShot(ghostMove[(_counter++ % 2)], 0.25f);
    }

    public void DemonJump()
    {
        _audio.PlayOneShot(demonJump[(_counter++ % 2)]);
    }

    public void DemonLand()
    {
        _audio.PlayOneShot(demonLand);
    }

    public void DemonCaster()
    {
        _audio.PlayOneShot(demonCaster);
    }
}
