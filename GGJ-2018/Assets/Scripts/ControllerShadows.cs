using System;
using System.Collections;
using UnityEngine;
using VRTK;

public class ControllerShadows : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        ;
        GameObject leftHand = null;
        while (leftHand == null)
        {
            leftHand = VRTK_DeviceFinder.GetControllerLeftHand(true);
            yield return null;
        }

        GameObject rightHand = null;
        while (rightHand == null)
        {
            rightHand = VRTK_DeviceFinder.GetControllerRightHand(true);
            yield return null;
        }

        yield return TurnOffShadows(leftHand);
        yield return TurnOffShadows(rightHand);
    }

    private IEnumerator TurnOffShadows(GameObject controller)
    {

        Renderer[] renderers = new Renderer[0];
        while (renderers.Length == 0)
        {
            renderers = controller.GetComponentsInChildren<Renderer>();
            yield return null;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}