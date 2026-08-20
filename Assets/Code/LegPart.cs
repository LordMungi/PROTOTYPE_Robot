using UnityEngine;

public class LegPart : MonoBehaviour
{
    public enum LegTypes
    {
        NONE,
        DoubleJump,
        Climb,
        Propeller
    }

    public LegTypes type;

    private Transform _defaultParent;
    private Rigidbody _body;

    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _defaultParent = transform.parent;

    }

    private void FixedUpdate()
    {
    }

    void Update()
    {

    }

    public void Grab(Transform parent)
    {
        transform.position = parent.position;
        transform.rotation = parent.rotation;
        transform.SetParent(parent);
        _body.isKinematic = true;
        _body.detectCollisions = false;
    }

    public void Release()
    {
        transform.SetParent(_defaultParent);
        _body.isKinematic = false;
        _body.detectCollisions = true;
    }
}
