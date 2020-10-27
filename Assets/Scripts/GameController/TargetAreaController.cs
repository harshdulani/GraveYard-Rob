using System.Collections;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;
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
    public int digsHitsRequired;

    private int _digHitsRemaining;
    private float _delta;

    private PlayerCombat _playerCombat;
    
    private void Start()
    {
        _digHitsRemaining = digsHitsRequired;
        var position = dirtHole.position;
        position = new Vector3(position.x, dirtStartY, position.z);
        
        dirtHole.position = position;
        _delta = Mathf.Abs(dirtEndY - dirtStartY) / digsHitsRequired;

        _playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
    }

    public void TargetGiveHit()
    {
        StartCoroutine(TargetTakeHit());
    }

    private IEnumerator TargetTakeHit()
    {
        yield return new WaitForSeconds(0.5f);
        
        var targetPercent = (--_digHitsRemaining) / (float)(digsHitsRequired);
        print(_digHitsRemaining);

        dirtHole.position = Vector3.MoveTowards(dirtHole.position, new Vector3(dirtHole.position.x, dirtEndY, dirtHole.position.z), _delta);

        for (var i = healthBarLeft.fillAmount; i >= targetPercent; i -= 0.05f)
        {
            UpdateHealthBar(i);
            yield return new WaitForSeconds(0.05f);
        }

        if (_digHitsRemaining == 0)
        {
            _playerCombat.isDiggingComplete = true;
            _playerCombat.IsAllowedToDig = false;
            Destroy(healthBarLeft.transform.parent.parent.gameObject);
            //change to modified grave with gold inside
            print("initiate looting grave");
        }
        //also make sure youre not allowed to dig anymore
        //either by some if else
        //or destroy targetarea gameobject hence this manager
    }
    
    private void UpdateHealthBar(float amount)
    {
        healthBarRight.fillAmount = healthBarLeft.fillAmount = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if(_digHitsRemaining >= 0)
            _playerCombat.IsAllowedToDig = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        if(_playerCombat.IsAllowedToDig)
            _playerCombat.IsAllowedToDig = false;
    }
}
