using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float lifetime = 3f;
    public float throwSpeed = 800f;
    public int projectileDamage = 100;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            ShootTowards(GameObject.FindGameObjectWithTag("Player").transform);
            Destroy(gameObject, lifetime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShootTowards(Transform target)
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * throwSpeed);
        //nulling this so that when the enemy points to new location of player, the projectile doesn't change positon due to local space
        transform.parent = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
