using UnityEngine;
using VRTK;

public class Book : VRTK_InteractableObject
{
    [SerializeField]
    private BookCover _cover = null;

    [SerializeField]
    private BookImage _image = null;

    [SerializeField]
    private Rigidbody _coverRigidbody = null;

    [SerializeField]
    private Collider _coverCollider = null;

    [SerializeField]
    private HingeJoint _joint;

    protected override void Awake()
    {
        base.Awake();

        _image.enabled = false;
    }

    protected override void Update()
    {
        base.Update();

        bool canInteract = Mathf.Abs(_joint.angle) >= 90f;
        if (!canInteract && _image.enabled)
        {
            if (_image.IsGrabbed())
            {
                _image.ForceStopInteracting();
            }
        }

        _image.enabled = canInteract;
    }
}