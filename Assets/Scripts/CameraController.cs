using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject playerReference;
    public GameObject cameraRoot;
    public GameObject cameraRotation;
    public GameObject cameraLook;
    // Update is called once per frame
    void LateUpdate()
    {
        cameraRoot.transform.position = playerReference.transform.position;
    }
}
