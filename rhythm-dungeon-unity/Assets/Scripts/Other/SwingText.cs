using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To make a text swing in desired direction.
public class SwingText : MonoBehaviour
{
    public float maxDistance;
    float originalY, distancePerFrame, direction;
    void Start()
    {
        originalY = transform.position.y;
        distancePerFrame = maxDistance / (1f * GameManager.Instance.secondPerHalfBeat)
                           * Time.fixedDeltaTime;
        direction = 1f;
    }
    void FixedUpdate()
    {
        transform.position += Vector3.up * distancePerFrame * direction;
        if (Mathf.Abs(transform.position.y - originalY) > maxDistance)
            direction = -direction;
    }
}
