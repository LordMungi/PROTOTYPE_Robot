using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float throwForce;


    [Header("Components")]
    [SerializeField] private Rigidbody body;
    [SerializeField] private Transform feet;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject legPartSlot;
    [SerializeField] private GameObject partStash;

    [Header("Parts")]
    [SerializeField] private LegPart currentLegPart;
    [SerializeField] private GameObject DoubleJumpLegs;
    [SerializeField] private GameObject RunLegs;
    [SerializeField] private GameObject PropellerLegs;
    [Space]
    [SerializeField] private GameObject GrabberArms;

    [Header("Elements")]
    [SerializeField] private GrabberHand Hand;

    public enum ArmTypes
    {
        Grabber,
        Shooter
    }

    [Header("Variables")]
    public float Speed;
    public float PropellerForce;
    public float Sensitivity;
    public float JumpForce;

    private LegPart _grabbedPart = null;
    private LegPart.LegTypes _currentLegType;
    private ArmTypes ArmType;

    private bool _jumped = false;
    private bool _doubleJumped = false;
    private bool _jumpEnabled = true;
    public LegPart _nearPart = null;

    void Start()
    {
        ServiceProvider.Instance.AddService<TaskScheduler>(new GameObject("TaskScheduler").AddComponent<TaskScheduler>());
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DoubleJumpLegs.SetActive(true);
        GrabberArms.SetActive(true);

        ArmType = ArmTypes.Grabber;
    }

    void Update()
    {

        #region Landing
        if (_jumpEnabled && Physics.Raycast(feet.position, -Vector3.up, 0.2f))
        {
            _jumped = false;
            _doubleJumped = false;
            _jumpEnabled = false;
            ServiceProvider.Instance.GetService<TaskScheduler>().Schedule(EnableJump, 1);
        }
        #endregion

        #region Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!_jumped)
            {
                body.AddForce(new Vector3(0, JumpForce, 0));
                _jumped = true;
            }
            else if (!_doubleJumped && _currentLegType == LegPart.LegTypes.DoubleJump)
            {
                body.AddForce(new Vector3(0, JumpForce, 0));
                _doubleJumped = true;
            }
        }
        if (Input.GetKey(KeyCode.Space) && _jumped && _currentLegType == LegPart.LegTypes.Propeller)
        {
            body.AddForce(new Vector3(0, PropellerForce * Time.deltaTime, 0));
        }
        #endregion

        #region Grab
        if (Input.GetKeyDown(KeyCode.E) && _nearPart)
        {
            if (_grabbedPart)
            {
                _grabbedPart.Release();
                _grabbedPart = null;
            }

            _grabbedPart = _nearPart;
            _nearPart.Grab(partStash.transform);
            _nearPart = null;
        }

        if (Input.GetKeyDown(KeyCode.Q) && (_grabbedPart || currentLegPart))
        {
            if (_grabbedPart)
            {
                _grabbedPart.Release();
                _grabbedPart = null;
            }
            else
            {
                currentLegPart.Release();
                currentLegPart = null;
                _currentLegType = LegPart.LegTypes.NONE;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ApplyPart();
        }
        #endregion

        #region Move
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Speed * Time.deltaTime);
        #endregion

        #region Throw
        if (Input.GetMouseButtonDown(0) && _grabbedPart)
        {
            _grabbedPart.Throw(throwForce);
            _grabbedPart = null;
        }
        #endregion

        /*
        #region Shoot
        if (Input.GetMouseButtonDown(0) && ArmType == ArmTypes.Grabber)
        {
            Hand.Shoot(cam.transform.forward);
            Debug.DrawRay(GrabberArms.transform.position, cam.transform.forward * 1000);
        }
        #endregion
        */

        #region Rotation
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Sensitivity);
        #endregion
    }

    void ApplyPart()
    {
        LegPart aux;


        if (_grabbedPart)
        {
            aux = _grabbedPart;
            if (currentLegPart)
            {
                _grabbedPart = currentLegPart;
                _grabbedPart.Grab(partStash.transform);
            } 
            else
                _grabbedPart = null;

            currentLegPart = aux;
            currentLegPart.Grab(legPartSlot.transform);
            _currentLegType = currentLegPart.type;

        }
        else if (currentLegPart)
        {
            _grabbedPart = currentLegPart;
            _grabbedPart.Grab(partStash.transform);
            currentLegPart = null;
            _currentLegType = LegPart.LegTypes.NONE;
        }


    }

    void EnableJump()
    {
        _jumpEnabled = true;
    }

    void DisableLegs()
    {
        DoubleJumpLegs.SetActive(false);
        RunLegs.SetActive(false);
        PropellerLegs.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PartArea"))
            _nearPart = other.transform.parent.gameObject.GetComponent<LegPart>();

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PartArea"))
            _nearPart = null;
    }
}
