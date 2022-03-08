using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Set a fixed horizontal FOV
    public float horizontalFOV = 120f;
    //somewhere in update if screen is resizable
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        cam.fieldOfView = calcVertivalFOV(horizontalFOV, Camera.main.aspect);
    }

    private float calcVertivalFOV(float hFOVInDeg, float aspectRatio)
    {
        float hFOVInRads = hFOVInDeg * Mathf.Deg2Rad;
        float vFOVInRads = 2 * Mathf.Atan(Mathf.Tan(hFOVInRads / 2) / aspectRatio);
        float vFOV = vFOVInRads * Mathf.Rad2Deg;
        return vFOV;
    }
}
