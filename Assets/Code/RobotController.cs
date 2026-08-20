using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody body;
    [SerializeField] private Transform feet;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject legPartSlot;

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

    public LegPart.LegTypes _currentLegType;
    private ArmTypes ArmType;

    private bool _jumped = false;
    private bool _doubleJumped = false;
    private bool _jumpEnabled = true;
    private LegPart _nearPart = null;

    void Start()
    {
        ServiceProvider.Instance.AddService<TaskScheduler>(new GameObject("TaskScheduler").AddComponent<TaskScheduler>());
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DoubleJumpLegs.SetActive(true);
        GrabberArms.SetActive(true);

        _currentLegType = currentLegPart.type;
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
            if (currentLegPart)
            {
                currentLegPart.Release();
                currentLegPart = null;
            }

            currentLegPart = _nearPart;
            _currentLegType = currentLegPart.type;
            _nearPart.Grab(legPartSlot.transform);
            _nearPart = null;
        }
        #endregion

        #region Release
        if (Input.GetKeyDown(KeyCode.Q) && currentLegPart)
        {
            currentLegPart.Release();
            currentLegPart = null;
            _currentLegType = LegPart.LegTypes.NONE;
        }
        #endregion

        #region Move
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Speed * Time.deltaTime);
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
            _nearPart = other.GetComponent<LegPart>();

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PartArea"))
            _nearPart = null;
    }
}
