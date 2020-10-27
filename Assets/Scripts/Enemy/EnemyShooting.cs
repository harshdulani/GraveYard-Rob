using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    //death //make class for this asap
    public delegate void OnEnemyDestroy(Transform enemyTransform);
    public OnEnemyDestroy destructionEvent;

    public Transform projectileSpawn;
    public GameObject projectilePrefab;
    public float shotInterval = 1f;
    public int bumpDamage = 50;

    public bool keepShooting = true;

    private int _c = 0;

    private void Start()
    {
        StartCoroutine("ShootYourShot");
        //should go in gamecontroller
        //FindObjectOfType<TargetingPlayer>().BirthNotify(this);
    }

    private IEnumerator ShootYourShot()
    {
        while(true)
        {
            yield return new WaitForSeconds(shotInterval);
            if (!keepShooting) continue;
            if (!(GameObject.FindGameObjectWithTag("Player")))
                ShotsFired();
        }
    }

    public void ShotsFired()
    {
        Instantiate(projectilePrefab, projectileSpawn).name = "projectile " + _c++;
    }

    private void OnDestroy()
    {
        //same as checking if destructionevent is null or not
        destructionEvent?.Invoke(transform);
    }
}
