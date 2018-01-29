using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Planet : VRTK_InteractableObject
{
    private Dictionary<VRTK_InteractableObject, bool> _prevValues = new Dictionary<VRTK_InteractableObject, bool>();

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed(currentGrabbingObject);

        _prevValues.Clear();

        VRTK_InteractableObject[] objects = GameObject.FindObjectsOfType<VRTK_InteractableObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == this)
                continue;

            if (!objects[i].IsGrabbed())
            {
                SetGravity(objects[i], false);
            }
            else
            {
                objects[i].InteractableObjectUngrabbed += HandleObjectDroppedNoGravity;
            }
        }
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed(previousGrabbingObject);

        VRTK_InteractableObject[] objects = GameObject.FindObjectsOfType<VRTK_InteractableObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == this)
                continue;

            if (!objects[i].IsGrabbed())
            {
                SetGravity(objects[i], true);
            }
            else
            {
                objects[i].InteractableObjectUngrabbed += HandleObjectDroppedGravity;
            }
        }
    }

    private void HandleObjectDroppedNoGravity(object sender, InteractableObjectEventArgs e)
    {
        VRTK_InteractableObject[] objs = e.interactingObject.GetComponentsInParent<VRTK_InteractableObject>();
        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].InteractableObjectUngrabbed -= HandleObjectDroppedNoGravity;
            SetGravity(objs[i], false);
        }
    }

    private void HandleObjectDroppedGravity(object sender, InteractableObjectEventArgs e)
    {
        VRTK_InteractableObject[] objs = e.interactingObject.GetComponentsInParent<VRTK_InteractableObject>();
        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].InteractableObjectUngrabbed -= HandleObjectDroppedGravity;
            SetGravity(objs[i], true);
        }
    }

    private void SetGravity(VRTK_InteractableObject obj, bool useGravity)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        if (!useGravity)
        {
            _prevValues.Add(obj, rb.useGravity);

            rb.useGravity = false;
            rb.AddForce(UnityEngine.Random.insideUnitSphere * 30f * rb.mass);
        }
        else
        {
            if (_prevValues.ContainsKey(obj))
            {
                rb.useGravity = _prevValues[obj];
            }
            else
            {
                rb.useGravity = true;
            }
        }
    }
}