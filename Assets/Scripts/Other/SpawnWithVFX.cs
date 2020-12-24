using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWithVFX : MonoBehaviour
{
    public GameObject vfx;

    public List<GameObject> toDisable;
    
    public float waitBeforeShowVFX, waitBeforeRender, waitBeforeStartFollowing;

    private void Start()
    {
        foreach (var renderer in toDisable)
        {
            renderer.SetActive(false);
        }
        StartCoroutine(SpawnWithDelay());
    }

    private IEnumerator SpawnWithDelay()
    {
        yield return new WaitForSeconds(waitBeforeShowVFX);

        Instantiate(vfx, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(waitBeforeRender);
        
        foreach (var renderer in toDisable)
        {
            renderer.SetActive(true);
        }
        
        yield return new WaitForSeconds(waitBeforeStartFollowing);

        StartCoroutine(GetComponent<EnemyFollow>().followMechanic);
    }
}