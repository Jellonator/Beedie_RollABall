using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    /// Reference to the player
    public GameObject playerReference;
    /// Reference to the camera's root
    public GameObject cameraRoot;
    /// Reference to the rotation object that is used to rotate the camera around the player.
    public GameObject cameraRotation;
    /// Reference to the look object that is used to determine rotation axis vectors.
    public GameObject cameraLook;

    // Update is called once per frame
    void LateUpdate()
    {
        // Update the camera to follow the player
        cameraRoot.transform.position = playerReference.transform.position;
    }
}
