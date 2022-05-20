using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformConstraint : MonoBehaviour
{
    Vector3 original_position = new Vector3();
    Vector3 original_rotation = new Vector3();
    private void Awake()
    {
        original_position = transform.localPosition;
        original_rotation = transform.localEulerAngles;
    }

    private void Update()
    {
        transform.localPosition = original_position;
        transform.localEulerAngles = original_rotation;
    }
}
