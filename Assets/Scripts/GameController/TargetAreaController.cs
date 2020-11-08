using System.Collections;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TargetAreaController : MonoBehaviour
{
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
        dirtHole.position = new Vector3(position.x, dirtStartY, position.z);
        
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

        var position = dirtHole.position;
        dirtHole.position = Vector3.MoveTowards(position, new Vector3(position.x, dirtEndY, position.z), _delta);

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
            //TODO change to modified grave with gold inside
            print("initiate looting grave");
        }
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
