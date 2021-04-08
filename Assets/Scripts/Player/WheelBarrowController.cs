using Cinemachine;
using UnityEngine;

public class WheelBarrowController : MonoBehaviour
{
    #region Singleton

    public static WheelBarrowController main;

    private void Awake()
    {
        if (!main)
            main = this;
        else
            Destroy(main);
    }
    

    #endregion

    public Transform liftedState;
    public GameObject tutorial;
    public CinemachineFreeLook tpsCam;

    public GameObject marker;

    public float lowSensi = 200;
    private float _normSensi;
    
    public bool isLifted;
    public bool hasMaxGold, canLift;
    
    private bool _myBarrowWalking;
    private float _originalYPos;
    
    private Animator _animator;
    private static readonly int BarrowLifted = Animator.StringToHash("barrowLifted");
    private static readonly int ValZ = Animator.StringToHash("valZ");
    private static readonly int BarrowWalking = Animator.StringToHash("barrowWalking");

    private void Start()
    {
        _animator = PlayerStats.main.GetComponent<Animator>();
        _originalYPos = transform.position.y;
        _normSensi = tpsCam.m_XAxis.m_MaxSpeed;
    }

    private void Update()
    {
        if (!isLifted) return;

        var valZ = _animator.GetFloat(ValZ);
        
        if (valZ >= 0.15f)
        {
            if (_myBarrowWalking) return;
            
            _animator.SetBool(BarrowWalking, true);
            _myBarrowWalking = true;
        }
        else
        {
            if (!_myBarrowWalking) return;
            
            _animator.SetBool(BarrowWalking, false);
            _myBarrowWalking = false;
        }
    }

    private void ToggleBarrowLifting()
    {
        isLifted = !isLifted;

        _animator.SetBool(BarrowLifted, isLifted);
        MovementInput.current.blockStrafing = isLifted;
        MovementInput.current.blockWalkBack = isLifted;
        MovementInput.current.blockJumping = isLifted;
        
        marker.SetActive(!isLifted);
        
        if (isLifted)
        {
            transform.SetParent(_animator.transform);
            transform.localPosition = liftedState.position;
            transform.localRotation = liftedState.rotation;
            
            tpsCam.m_XAxis.m_MaxSpeed = lowSensi;
        }
        else
        {
            transform.SetParent(null);
            transform.Rotate(-transform.rotation.eulerAngles.x, 0, 0);
            transform.position = new Vector3(transform.position.x, _originalYPos, transform.position.z);
            
            tpsCam.m_XAxis.m_MaxSpeed = _normSensi;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;
        if(isLifted) return;
        if(!hasMaxGold) return;

        canLift = true;
        tutorial.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        if(!canLift) return;
        
        if (Input.GetButtonDown("Fire3"))
            ToggleBarrowLifting();
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        canLift = false;
        tutorial.SetActive(false);
    }
}
