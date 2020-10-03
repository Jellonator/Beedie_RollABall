﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    /// Reference to compass (the UI object that is rotated)
    public GameObject compassReference;
    /// Reference to the Camera itself
    public GameObject cameraReference;
    /// Reference to the Camera's parent
    public GameObject cameraParentReference;
    /// Reference to the player
    public GameObject playerReference;
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
        // Previous view normal
        Vector3 prev_dir = transform.rotation * Vector3.up;
        // When the player looks left or right, rotate around the Y view axis (does not affect gravity)
        transform.RotateAround(Vector3.zero, y_axis, look.x);
        // When the player rotates up or down, rotate around the X view axis
        transform.RotateAround(Vector3.zero, x_axis, -movement.y);
        // When the player rotates left or right, rotate around the Z view axis
        transform.RotateAround(Vector3.zero, z_axis, movement.x);
        // When the player rotates the view such that:
        //  * The new rotation is approaching the player's ground normal
        //  * the new rotation is sufficiently close to the ground normal,
        // Make the rotation approach the actual ground normal.
        if (movement.magnitude > 1e-3) {
            // Current view normal
            Vector3 new_dir = transform.rotation * Vector3.up;
            // Raycast to get the player's ground normal.
            RaycastHit hit;
            // Disable lock
            shouldLock = false;
            if (Physics.Raycast(playerReference.transform.position, Physics.gravity.normalized, out hit, 0.75f)) {
                // Compare dot products with the player's ground normal.
                // Check that the new view normal is similar to the ground normal,
                // and that it's closer than the previous view normal.
                float prev_dot = Vector3.Dot(hit.normal, prev_dir);
                float new_dot = Vector3.Dot(hit.normal, new_dir);
                if (new_dot > 0.99f && (new_dot > prev_dot || new_dot > 0.999f)) {
                    // Enable lock
                    shouldLock = true;
                    lockDirection = hit.normal;
                }
            }
        // Only lock if player is not giving inputs
        } else if (shouldLock) {
            // Get the target rotation (lock direction is UP vector)
            Quaternion target = Quaternion.LookRotation(z_axis, lockDirection);
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
        cameraParentReference.transform.rotation = transform.rotation;
        // Rotate gravity
        Physics.gravity = transform.rotation * baseGravity;
        // Rotate the compass
        compassReference.transform.rotation = Quaternion.Inverse(transform.rotation);
    }
}
