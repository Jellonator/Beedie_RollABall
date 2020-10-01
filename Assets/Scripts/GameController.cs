using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    // Reference to compass
    public GameObject compassReference;
    // Reference to the Camera itself
    public GameObject cameraReference;
    // Reference to the Camera's parent
    public GameObject cameraParentReference;
    // Base gravity direction (points down, will be rotated)
    private Vector3 baseGravity = new Vector3(0f, -1f, 0f) * 25.0f;
    // Movement pulled from OnMove
    private Vector2 movement = Vector2.zero;
    // Look value pulled from OnLook
    private Vector2 look = Vector2.zero;

    // Handle inputs
    void OnMove(InputValue movementValue)
    {
        movement = movementValue.Get<Vector2>();
    }

    void OnLook(InputValue lookValue)
    {
        look = lookValue.Get<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the view axis to rotate the game by
        Vector3 x_axis = cameraReference.transform.right;
        Vector3 y_axis = cameraReference.transform.up;
        Vector3 z_axis = cameraReference.transform.forward;
        // Rotate the game around the axis
        transform.RotateAround(Vector3.zero, y_axis, look.x);
        transform.RotateAround(Vector3.zero, x_axis, -movement.y);
        transform.RotateAround(Vector3.zero, z_axis, movement.x);
        // Apply the rotation
        cameraParentReference.transform.rotation = transform.rotation;
        Physics.gravity = transform.rotation * baseGravity;
        compassReference.transform.rotation = Quaternion.Inverse(transform.rotation);
    }
}
