using UnityEngine;

public class GrabberHand : MonoBehaviour
{
    public float Speed = 50;

    [SerializeField] Transform Parent;

    private bool _isFlying;
    private Vector3 _direction;

    private Vector3 _parentOffset;

    void Start()
    {
        _parentOffset = transform.position - Parent.position;
    }

    void Update()
    {
        if (_isFlying)
            transform.Translate(_direction * Speed * Time.deltaTime);

        else
        {
            transform.position = Parent.position + _parentOffset;
            transform.RotateAround(Parent.position, Parent.rotation.x);
        }
    }

    public void Shoot(Vector3 direction)
    {
        _isFlying = true;
        _direction = direction;
    }
}
