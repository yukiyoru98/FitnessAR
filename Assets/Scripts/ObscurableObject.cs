using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObscurableObject : MonoBehaviour
{
    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.material.renderQueue = 3002;
        }
    }
}
