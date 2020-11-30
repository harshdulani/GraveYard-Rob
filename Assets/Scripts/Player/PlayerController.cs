using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CinemachineVirtualCamera climbDownFenceCamera;
    public CinemachineFreeLook tpsCamera;

    private PlayerWeaponController _weaponController;
    
    private static readonly int PlayerBorn = Animator.StringToHash("playerBorn");

    private void Start()
    {
        _weaponController = GetComponentInChildren<PlayerWeaponController>();
        _weaponController.gameObject.SetActive(false);
        PlayerEvents.current.InvokePlayerBirth();
        
        PlayerEvents.current.InvokeHealthChange();

        //so that hes not visible on main menu
        transform.localScale = new Vector3(0, 0, transform.localScale.z);
    }

    private void OnEnable()
    {
        GameFlowEvents.current.gameplayStart += OnGameplayStart;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameplayStart -= OnGameplayStart;
    }

    private void OnGameplayStart()
    {
        transform.localScale = Vector3.one;
        GetComponent<Animator>().SetBool(PlayerBorn, true);
    }

    public void OnClimbDownFence()
    {
        GetComponent<PlayerCombat>().SwapWeapon();
        //so that attacks can happen
        GameStats.current.isGamePlaying = true;

        climbDownFenceCamera.gameObject.SetActive(false);
        tpsCamera.gameObject.SetActive(true);
    }

    public void DecreaseHealth(int amt)
    {
        if (PlayerStats.main.TakeHit(amt))
        {
            //die
            print("YOU DIED.");
            PlayerEvents.current.InvokePlayerDeath();
            Destroy(gameObject);
        }
        
        PlayerEvents.current.InvokeHealthChange();
    }

    

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.CompareTag("Projectile"))
        {
            DecreaseHealth(hit.gameObject.GetComponent<ProjectileController>().projectileDamage);
            print("projectile hit w " + hit.gameObject.name);
            Destroy(hit.gameObject);
        }
    }
}