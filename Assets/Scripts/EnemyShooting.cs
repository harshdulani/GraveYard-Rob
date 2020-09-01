using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Transform projectileSpawn;
    public GameObject projectilePrefab;
    public float shotInterval = 1f;
    public int bumpDamage = 50;

    private int c = 0;

    private void Start()
    {
        StartCoroutine("ShootYourShot");
    }

    private IEnumerator ShootYourShot()
    {
        while(true)
        {
            yield return new WaitForSeconds(shotInterval);
            if(GameObject.FindGameObjectWithTag("Player") != null)
                ShotsFired();
        }
    }

    public void ShotsFired()
    {
        Instantiate(projectilePrefab, projectileSpawn).name = "projectile " + c++;
    }
}
