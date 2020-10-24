using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TargetAreaController : MonoBehaviour
{
    //TO DO: elevate the dirthole (and maybe coffin) using lerp
    //with the lerp rate being the percentage of change in prev _currentHealth to new _currentHealth
    //as compared to the distance between dirthole's initial pos and final pos
    //which should be calculate at start via a coroutine so that it doesnt slow down the start by a lot of calculations
    //or maybe calculate every time there is a new dig call

    [SerializeField]
    private Transform dirtHole;

    [SerializeField] 
    private float dirtStartY;
    [SerializeField] 
    private float dirtEndY;

    [SerializeField]
    private Image healthBarLeft, healthBarRight;
    public int maxHealth;

    private int _currentHealth;
    private float lerpDelta;

    private void Start()
    {
        _currentHealth = maxHealth;
        var position = dirtHole.position;
        position = new Vector3(position.x, dirtStartY, position.z);
        
        dirtHole.position = position;
    }

    public void TargetGiveHit(int hitHealth)
    {
        StartCoroutine(TargetTakeHit(hitHealth));
    }

    private IEnumerator TargetTakeHit(int hitHealth)
    {
        var getToHealth = _currentHealth - hitHealth;
        
        if(lerpDelta == 0f)
            lerpDelta = (_currentHealth - getToHealth) / (float) maxHealth;
        
        yield return new WaitForSeconds(0.5f);
        dirtHole.position = Vector3.MoveTowards(dirtHole.position, new Vector3(dirtHole.position.x, dirtEndY, dirtHole.position.z), 0.2f);
        
        for (int i = _currentHealth; i >= getToHealth; i -= hitHealth / 10)
        {
            _currentHealth = i;
            UpdateHealthBar();
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    private void UpdateHealthBar()
    {
        healthBarRight.fillAmount = healthBarLeft.fillAmount = (float)(_currentHealth) / (float)(maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerCombat>().allowedToDig = true;
            //play animation for swap weapon with shovel
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.GetComponent<PlayerCombat>().allowedToDig = false;
        //play animation for swap shovel with weapon
    }
}
