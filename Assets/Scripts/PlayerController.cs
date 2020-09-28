using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue movementValue)
    {
        movement = movementValue.Get<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 movementVector = new Vector3(movement.x, 0.0f, movement.y);
        rigidbody.AddForce(movementVector);
    }
}
