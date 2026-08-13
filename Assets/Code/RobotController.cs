using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private Transform FeetOrigin;
    [SerializeField] private GameObject DoubleJumpLegs;
    [SerializeField] private GameObject RunLegs;
    [SerializeField] private GameObject PropellerLegs;
    public enum LegTypes
    {
        DoubleJump,
        Run,
        Propeller
    }

    public float Speed;
    public float PropellerForce;
    public float Sensitivity;
    public float JumpForce;

    public LegTypes LegType;

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

        LegType = LegTypes.DoubleJump;
    }

    void Update()
    {
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

        if (_jumpEnabled && Physics.Raycast(FeetOrigin.position, -Vector3.up, 0.2f))
        {
            _jumped = false;
            _doubleJumped = false;
            _jumpEnabled = false;
            ServiceProvider.Instance.GetService<TaskScheduler>().Schedule(EnableJump, 1);
        }

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

        if (Input.GetKey(KeyCode.LeftShift) && LegType == LegTypes.Run)
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Speed * 2 * Time.deltaTime);
        else
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Speed * Time.deltaTime);

        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Sensitivity);
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
