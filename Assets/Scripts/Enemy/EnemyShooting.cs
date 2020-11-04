using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Transform projectileSpawn;
    public GameObject projectilePrefab;
    public float shotInterval = 1f;
    public int bumpDamage = 50;

    public bool keepShooting = true;

    private int _projectileCounter = 0;
    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ShootYourShot());
    }

    private IEnumerator ShootYourShot()
    {
        while(true)
        {
            yield return new WaitForSeconds(shotInterval);
            if (!keepShooting) continue;
            if (!_player)
                ShotsFired();
        }
    }

    private void ShotsFired()
    {
        Instantiate(projectilePrefab, projectileSpawn).name = "projectile " + _projectileCounter++;
    }
}
