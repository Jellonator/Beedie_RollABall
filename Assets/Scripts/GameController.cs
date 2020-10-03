using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    /// Reference to compass (the UI object that is rotated)
    public GameObject compassReference;
    /// Reference to the Camera itself
    public GameObject cameraReference;
    /// Reference to camera's controller
    private CameraController cameraController;
    /// Reference to the player
    public GameObject playerReference;
    /// Reference to PlayerController
    private PlayerController playerController;
    /// Base gravity direction (points down, will be rotated)
    private Vector3 baseGravity = new Vector3(0f, -1f, 0f) * 25.0f;
    /// Movement pulled from OnMove
    private Vector2 movement = Vector2.zero;
    /// Look value pulled from OnLook
    private Vector2 look = Vector2.zero;
    /// True if view should lock to given normal
    private bool shouldLock = false;
    /// The normal to rotate towards when locking
    private Vector3 lockDirection = Vector3.up;
    /// Mouse movement
    private Vector2 mouseMovement = Vector2.zero;
    /// Mouse down
    private bool mouseDown = false;
    /// Mouse offset
    private Vector2 mouseOffset = Vector2.zero;
    /// View rotation
    private float viewRotation = 45f;

    void Start()
    {
        if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.Linux || !Application.isEditor) {
            // for some reason this specifically does not work in the Linux editor
            // https://issuetracker.unity3d.com/issues/linux-inputsystems-mouse-delta-values-do-not-change-when-the-cursor-lockstate-is-set-to-locked
            Cursor.lockState = CursorLockMode.Locked;
        }
        playerController = playerReference.GetComponent<PlayerController>();
        cameraController = cameraReference.GetComponent<CameraController>();
    }

    // Handle inputs
    void OnMove(InputValue movementValue)
    {
        movement = movementValue.Get<Vector2>();
    }

    void OnLook(InputValue lookValue)
    {
        look = lookValue.Get<Vector2>();
    }

    void OnMouseDown(InputValue mouseValue) {
        mouseDown = mouseValue.Get<float>() > 0.5f;
    }

    void OnMouseLook(InputValue mouseValue) {
        mouseMovement = mouseValue.Get<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        // Can't use MouseLook event because it doesn't understand mouse lock
        // mouseMovement = new Vector2(
        //     Input.GetAxis("Mouse X"),
        //     Input.GetAxis("Mouse Y")
        // );
        // Get the view axis to rotate the game by
        Vector3 x_axis = cameraController.cameraLook.transform.right;
        Vector3 y_axis = cameraController.cameraLook.transform.up;
        Vector3 z_axis = cameraController.cameraLook.transform.forward;
        // Previous view normal
        Vector3 prev_dir = transform.rotation * Vector3.up;
        // When the player looks left or right, rotate around the Y view axis (does not affect gravity)
        transform.RotateAround(Vector3.zero, y_axis, look.x);
        // When the player rotates up or down, rotate around the X view axis
        transform.RotateAround(Vector3.zero, x_axis, -movement.y);
        // When the player rotates left or right, rotate around the Z view axis
        transform.RotateAround(Vector3.zero, z_axis, movement.x);
        // When player looks up or down, rotate view
        viewRotation = Mathf.Clamp(viewRotation - look.y, -90f, 90f);
        // mouse movement, similar to above
        if (mouseDown) {
            transform.RotateAround(Vector3.zero, x_axis, -mouseMovement.y * 0.25f);
            transform.RotateAround(Vector3.zero, z_axis, mouseMovement.x * 0.25f);
        } else {
            transform.RotateAround(Vector3.zero, y_axis, mouseMovement.x * 0.25f);
            viewRotation = Mathf.Clamp(viewRotation - mouseMovement.y * 0.25f, -90f, 90f);
        }
        // When the player rotates the view such that:
        //  * The new rotation is approaching the player's ground normal
        //  * the new rotation is sufficiently close to the ground normal,
        // Make the rotation approach the actual ground normal.
        if (movement.magnitude > 1e-3 || (mouseDown && mouseMovement.magnitude > 1e-3)) {
            // Current view normal
            Vector3 new_dir = transform.rotation * Vector3.up;
            // Disable lock
            shouldLock = false;
            if (playerController.isOnGround) {
                // Compare dot products with the player's ground normal.
                // Check that the new view normal is similar to the ground normal,
                // and that it's closer than the previous view normal.
                float prev_dot = Vector3.Dot(playerController.groundNormal, prev_dir);
                float new_dot = Vector3.Dot(playerController.groundNormal, new_dir);
                if (new_dot > 0.99f && (new_dot > prev_dot || new_dot > 0.999f)) {
                    // Enable lock
                    shouldLock = true;
                    lockDirection = playerController.groundNormal;
                }
            }
        // Only lock if player is not giving inputs
        } else if (shouldLock && !mouseDown) {
            // Get the target rotation (lock direction is UP vector)
            // Correct the forward direction by calculating the right direction,
            // then using that to calculate a new forward direction
            Vector3 right = Vector3.Cross(lockDirection, z_axis);
            Vector3 forward = Vector3.Cross(right, lockDirection);
            Quaternion target = Quaternion.LookRotation(forward, lockDirection);
            // Get the difference between the target view normal and the current view normal
            float diff = Quaternion.Angle(transform.rotation, target);
            if (diff <= Time.deltaTime * 10f) {
                // Diff is so low, just lock completely.
                shouldLock = false;
                transform.rotation = target;
            } else {
                // Determine lerp amount, should rotate 10deg per second
                float amount = (10f * Time.deltaTime) / diff;
                if (amount >= 1.0f) {
                    // Clamp amount to 1.0f, and stop locking
                    amount = 1.0f;
                    shouldLock = false;
                }
                // Actual rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, target, amount);
            }
        }
        // Rotate the camera
        cameraController.cameraRoot.transform.rotation = transform.rotation;
        // Rotate gravity
        Physics.gravity = transform.rotation * baseGravity;
        // Rotate the compass
        compassReference.transform.rotation = Quaternion.Inverse(transform.rotation);
        // Set view rotation
        cameraController.cameraRotation.transform.localEulerAngles = new Vector3(viewRotation, 0, 0);
    }
}
