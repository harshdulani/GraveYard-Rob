using System.Collections;
using UnityEngine;
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

    [Header("Slide In Grave Canvas")]
    public SlideIntoScreen graveCanvas;
    private bool _hasFoundGrave;
    
    private int _digHitsRemaining;
    private float _delta;

    [Header("Gold Steal Canvas")] 
    public SlideIntoScreen goldCanvas;

    public Image goldBarLeft, goldBarRight;
    
    public Transform wheelBarrow, wheelBarrowDest;
    public int goldBricksRequired = 7;

    private int _goldBricksRemaining;

    [Header("Marker")] 
    public GameObject marker;
    public Text tutorialText;

    private bool _hasDugOnce;

    private PlayerCombat _playerCombat;
    private TargetingEnemyByTrigger _targeter;

    private readonly WaitForSeconds _waitHalfSec = new WaitForSeconds(0.5f);
    private readonly WaitForSeconds _waitUpdateCanvas = new WaitForSeconds(0.05f);

    private void OnEnable()
    {
        GameFlowEvents.current.gameplayStart += OnGameplayStart;
        GameFlowEvents.current.gameOver += OnGameOver;
        GameFlowEvents.current.gameplayPause += OnPause;
        GameFlowEvents.current.gameplayResume += OnResume;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameplayStart -= OnGameplayStart;
        GameFlowEvents.current.gameOver -= OnGameOver;
        GameFlowEvents.current.gameplayPause -= OnPause;
        GameFlowEvents.current.gameplayResume -= OnResume;
    }

    private void Start()
    {
        _digHitsRemaining = digsHitsRequired;
        _goldBricksRemaining = goldBricksRequired;
        
        _delta = Mathf.Abs(dirtEndY - dirtStartY) / digsHitsRequired;

        _playerCombat = PlayerStats.main.GetComponent<PlayerCombat>();
        _targeter = _playerCombat.GetComponentInChildren<TargetingEnemyByTrigger>();
    }

    public void TargetGiveHit(bool stealingGold = false)
    {
        if (stealingGold)
            StartCoroutine(TargetTakeGoldSteal());
        else
            StartCoroutine(TargetTakeHit());
    }

    private IEnumerator TargetTakeHit()
    {
        yield return _waitHalfSec;

        if (!_hasDugOnce)
        {
            _hasDugOnce = true;
            marker.SetActive(false);
        }
        
        var targetPercent = (--_digHitsRemaining) / (float)(digsHitsRequired);

        var position = dirtHole.position;
        dirtHole.position = new Vector3(position.x, Mathf.Clamp(position.y + _delta, dirtStartY, dirtEndY), position.z);

        for (var i = healthBarLeft.fillAmount; i >= targetPercent; i -= 0.05f)
        {
            UpdateHealthBar(i);
            yield return _waitUpdateCanvas;
        }

        if (_digHitsRemaining > 0) yield break;

        _playerCombat.isStealingGold = true;
        _playerCombat.IsAllowedToDig = true;
            
        //destroying grave dig canvas here
        Destroy(healthBarLeft.transform.parent.parent.gameObject);

        goldCanvas.transform.parent.gameObject.SetActive(true);

        dirtHole.GetChild(0).gameObject.SetActive(false);
        dirtHole.GetChild(1).gameObject.SetActive(true);

        wheelBarrow.SetPositionAndRotation(wheelBarrowDest.position, wheelBarrowDest.rotation);
        
        tutorialText.text = "Press LMB to Steal Gold";

        GameFlowEvents.current.InvokeUpdateObjective();
    }

    private IEnumerator TargetTakeGoldSteal()
    {
        _targeter.FaceGraveToStealGold(transform);
        
        yield return _waitHalfSec;
        
        var targetPercent = (--_goldBricksRemaining) / (float)(goldBricksRequired);

        for (var i = goldBarLeft.fillAmount; i >= targetPercent; i -= 0.05f)
        {
            UpdateGoldBar(i);
            yield return _waitUpdateCanvas;
        }
        
        yield return _waitHalfSec;
        yield return _waitHalfSec;
        
        //when you change number of gold bricks also change the order of the bricks in the prefab
        wheelBarrow.transform.GetChild(_goldBricksRemaining + 1).gameObject.SetActive(true);
        _playerCombat.goldBrick.SetActive(false);
        
        StartCoroutine(StopRotation());
        
        if (_goldBricksRemaining > 0) yield break;
        
        _playerCombat.isDoneStealingGold = true;
        _playerCombat.IsAllowedToDig = false;

        //destroying gold canvas here
        Destroy(goldCanvas.transform.parent.gameObject);
        
        WheelBarrowController.main.hasMaxGold = true;
        WheelBarrowController.main.marker.SetActive(true);
        
        GameFlowEvents.current.InvokeUpdateObjective();
    }

    private IEnumerator StopRotation()
    {
        yield return _waitHalfSec;
        yield return _waitHalfSec;
        _targeter.StopFacingGrave();
    }

    private void OnGameplayStart()
    {
        var position = dirtHole.position;
        dirtHole.position = new Vector3(position.x, dirtStartY, position.z);
    }
    
    private void UpdateHealthBar(float amount)
    {
        healthBarRight.fillAmount = healthBarLeft.fillAmount = amount;
    }
    
    private void UpdateGoldBar(float amount)
    {
        goldBarRight.fillAmount = goldBarLeft.fillAmount = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        if (!_hasFoundGrave)
        {
            _hasFoundGrave = true;
            GameFlowEvents.current.InvokeUpdateObjective();
            graveCanvas.StartSliding();
            marker.SetActive(true);
        }
        
        _playerCombat.IsAllowedToDig = true;
        tutorialText.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (_playerCombat.IsAllowedToDig)
        {
            _playerCombat.IsAllowedToDig = false;
            tutorialText.enabled = false;
        }
    }

    private void OnGameOver()
    {
        if(graveCanvas)
            graveCanvas.gameObject.SetActive(false);
    }
    
    private void OnPause()
    {
        graveCanvas.gameObject.SetActive(false);
    }

    private void OnResume()
    {
        graveCanvas.gameObject.SetActive(true);
    }
}
