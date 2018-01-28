using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneGizmo : MonoBehaviour
{
    [SerializeField]
    private Transform _root;

    private void OnDrawGizmos()
    {
        DrawGizmos(_root);
    }

    private void DrawGizmos(Transform t)
    {
        if (t.GetComponent<Collider>() != null)
            return;

        Gizmos.DrawSphere(t.position, 0.005f);

        foreach (Transform child in t)
        {
            DrawGizmos(child);
        }
    }
}