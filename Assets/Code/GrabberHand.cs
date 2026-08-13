using UnityEngine;

public class GrabberHand : MonoBehaviour
{
    public float Speed = 50;
    public float ReturnSpeed = 50;

    [SerializeField] Transform Parent;
    [SerializeField] Rigidbody Body;

    private bool _isGoing;
    private bool _isReturning;
    private Vector3 _direction;

    private Vector3 _parentOffset;

    private GameObject _grabbedObject;


    void Start()
    {
        _parentOffset = transform.position - Parent.position;
    }

    private void FixedUpdate()
    {
        if (_isGoing)
            Body.MovePosition(transform.position + _direction * Speed * Time.deltaTime);
        else if (_isReturning)
        {
            Body.MovePosition(Vector3.MoveTowards(transform.position, Parent.position + _parentOffset, ReturnSpeed * Time.deltaTime));
        }
    }

    void Update()
    {
        if (!_isGoing && !_isReturning)
            transform.position = Parent.position + _parentOffset;
        else if (_isReturning)
        {
            if (_grabbedObject)
                _grabbedObject.transform.position = transform.position;
        }
    }

    public void Shoot(Vector3 direction)
    {
        if (!_isGoing && !_isReturning)
        {
            _isGoing = true;
            _isReturning = false;
            _direction = direction;

            ServiceProvider.Instance.GetService<TaskScheduler>().Schedule(Return, 2);
        }
    }

    public void Return()
    {
        if (_isGoing)
        {
            _isGoing = false;
            _isReturning = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("A");
        if (other.CompareTag("Player"))
        {
            _isGoing = false;
            _isReturning = false;
            Destroy(_grabbedObject);
            _grabbedObject = null;
        }

        else
        {
            Return();
            if (other.CompareTag("Grabbable"))
            {
                _grabbedObject = other.gameObject;
            }
        }
    }
}
