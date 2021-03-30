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

    public Transform lifted;
    public GameObject tutorial;

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

        if (isLifted)
        {
            _animator.SetBool(BarrowLifted, true);
            MovementInput.current.blockStrafing = true;
            MovementInput.current.blockWalkBack = true;
            MovementInput.current.blockJumping = true;
            
            transform.SetParent(_animator.transform);
            transform.localPosition = lifted.position;
            transform.localRotation = lifted.rotation;
        }
        else
        {
            _animator.SetBool(BarrowLifted, false);
            MovementInput.current.blockStrafing = false;
            MovementInput.current.blockWalkBack = false;
            MovementInput.current.blockJumping = false;
            
            transform.SetParent(null);
            transform.Rotate(-transform.rotation.eulerAngles.x, 0, 0);
            transform.position = new Vector3(transform.position.x, _originalYPos, transform.position.z);
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
