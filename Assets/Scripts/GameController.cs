using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public GameObject cameraReference;

    // Update is called once per frame
    void OnMove(InputValue movementValue)
    {
        Vector2 movement = movementValue.Get<Vector2>();
        Physics.gravity = new Vector3(movement.x, -1.0f, movement.y).normalized * 9.8f;
    }
}
