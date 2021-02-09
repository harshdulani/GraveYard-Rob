using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWithVFX : MonoBehaviour
{
    public GameObject vfx;

    public List<GameObject> toDisable;

    public float waitBeforeRender, startFollowingAfterVFXDurationMultiplier = 0.35f;
    
    private float _waitBeforeStartFollowing;

    private static WaitForSeconds _waitForRender, _waitToStartFollow;

    private void Start()
    {
        _waitForRender = new WaitForSeconds(waitBeforeRender);
        
        foreach (var renderer in toDisable)
        {
            renderer.SetActive(false);
        }
        StartCoroutine(SpawnWithDelays());
    }

    private IEnumerator SpawnWithDelays()
    {
        var instance = Instantiate(vfx, transform.position, Quaternion.identity);
        
        instance.transform.position = new Vector3(transform.position.x, vfx.transform.position.y, transform.position.z);

        _waitBeforeStartFollowing = instance.transform.GetChild(0).GetComponent<ParticleSystem>().main.duration * startFollowingAfterVFXDurationMultiplier;
        
        yield return _waitForRender;
        
        foreach (var myRenderer in toDisable)
        {
            myRenderer.SetActive(true);
        }

        if(_waitToStartFollow == null)
            _waitToStartFollow = new WaitForSeconds(_waitBeforeStartFollowing);

        yield return _waitToStartFollow; 

        if(TryGetComponent(out EnemyFollow follow))
            StartCoroutine(follow.followMechanic);
        
        else if(TryGetComponent(out EnemyDiagonalMovement movement))
            movement.StartMoving();

        Destroy(instance, 5f);
    }
}