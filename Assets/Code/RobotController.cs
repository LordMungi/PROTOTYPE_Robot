using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody body;
    [SerializeField] private Transform feet;
    [SerializeField] private Camera cam;

    [Header("Parts")]
    [SerializeField] private GameObject DoubleJumpLegs;
    [SerializeField] private GameObject RunLegs;
    [SerializeField] private GameObject PropellerLegs;
    [Space]
    [SerializeField] private GameObject GrabberArms;

    [Header("Elements")]
    [SerializeField] private GrabberHand Hand;

    public enum LegTypes
    {
        DoubleJump,
        Run,
        Propeller
    }
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

    private LegTypes LegType;
    private ArmTypes ArmType;

    private bool _jumped = false;
    private bool _doubleJumped = false;
    private bool _jumpEnabled = true;
    private bool _isInTaller = false;

    void Start()
    {
        ServiceProvider.Instance.AddService<TaskScheduler>(new GameObject("TaskScheduler").AddComponent<TaskScheduler>());
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DoubleJumpLegs.SetActive(true);
        GrabberArms.SetActive(true);

        LegType = LegTypes.DoubleJump;
        ArmType = ArmTypes.Grabber;
    }

    void Update()
    {
        #region Taller
        if (_isInTaller)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                DisableLegs();
                DoubleJumpLegs.SetActive(true);
                LegType = LegTypes.DoubleJump;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                DisableLegs();
                RunLegs.SetActive(true);
                LegType = LegTypes.Run;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                DisableLegs();
                PropellerLegs.SetActive(true);
                LegType = LegTypes.Propeller;
            }
        }
        #endregion

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
            else if (!_doubleJumped && LegType == LegTypes.DoubleJump)
            {
                body.AddForce(new Vector3(0, JumpForce, 0));
                _doubleJumped = true;
            }
        }
        if (Input.GetKey(KeyCode.Space) && _jumped && LegType == LegTypes.Propeller)
        {
            body.AddForce(new Vector3(0, PropellerForce * Time.deltaTime, 0));
        }
        #endregion

        #region Move
        if (Input.GetKey(KeyCode.LeftShift) && LegType == LegTypes.Run)
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Speed * 2 * Time.deltaTime);
        else
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Speed * Time.deltaTime);
        #endregion

        #region Shoot
        if (Input.GetMouseButtonDown(0) && ArmType == ArmTypes.Grabber)
        {
            Hand.Shoot(cam.transform.forward);
            Debug.DrawRay(GrabberArms.transform.position, cam.transform.forward * 1000);
        }
        #endregion

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
        if (other.transform.CompareTag("Area"))
            _isInTaller = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Area"))
            _isInTaller = false;
    }
}
