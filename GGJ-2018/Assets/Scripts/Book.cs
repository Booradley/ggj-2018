using UnityEngine;
using VRTK;

public class Book : VRTK_InteractableObject
{
    [SerializeField]
    private BookCover _cover = null;

    [SerializeField]
    private Rigidbody _coverRigidbody = null;

    [SerializeField]
    private Collider _coverCollider = null;

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject)
    {
        base.Grabbed(currentGrabbingObject);
        
        ToggleCover(true);
        
        if (VRTK_DeviceFinder.GetControllerHand(currentGrabbingObject.controllerEvents.gameObject) == SDK_BaseController.ControllerHand.Left)
        {
            allowedTouchControllers = AllowedController.LeftOnly;
            allowedUseControllers = AllowedController.LeftOnly;

            _cover.allowedGrabControllers = AllowedController.RightOnly;
        }
        else if (VRTK_DeviceFinder.GetControllerHand(currentGrabbingObject.controllerEvents.gameObject) == SDK_BaseController.ControllerHand.Right)
        {
            allowedTouchControllers = AllowedController.RightOnly;
            allowedUseControllers = AllowedController.RightOnly;

            _cover.allowedGrabControllers = AllowedController.LeftOnly;
        }
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);

        ToggleCover(false);
        
        allowedTouchControllers = AllowedController.Both;
        allowedUseControllers = AllowedController.Both;

        _cover.allowedGrabControllers = AllowedController.Both;
    }

    private void ToggleCover(bool state)
    {
        if (!state)
        {
            _cover.ForceStopInteracting();
        }

        _cover.enabled = state;
        _cover.isGrabbable = state;
        //_coverRigidbody.isKinematic = state;
        //_coverCollider.isTrigger = state;
    }
}