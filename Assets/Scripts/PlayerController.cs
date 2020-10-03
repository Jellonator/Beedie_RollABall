using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    /// Count text object in UI
    public TextMeshProUGUI countText;
    /// Win text object in UI
    public GameObject winTextObject;
    /// The amount of time that the player may be OOB before restarting a level
    public float outOfBoundsTime = 2.5f;
    /// Player's Rigidbody component
    private Rigidbody rb;
    /// The number of collectibles that the player has collected
    private int count = 0;
    /// Track the number of walls that the player is touching
    private int numCollisions = 0;
    /// Timer used to track the amount of time that the player is considered OOB
    private float oobTimer = 0.0f;
    /// Player's current ground normal (if isOnGround is true)
    [HideInInspector]
    public Vector3 groundNormal = Vector3.up;
    /// Whether player is on the ground
    [HideInInspector]
    public bool isOnGround = true;
    /// Drag value while in air
    public float dragAir = 0f;
    /// Angular drag value while in air
    public float angularDragAir = 0f;
    /// Drag value while on the ground
    public float dragGround = 0.25f;
    /// Angular drag value while on the ground
    public float angularDragGround = 0.2f;
    /// Drag value while player is trying to stop
    public float dragStop = 1.2f;
    /// Angular drag value while player is trying to stop
    public float angularDragStop = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetCountText();
        winTextObject.SetActive(false);
    }

    /// Update the counter's text
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12) {
            winTextObject.SetActive(true);
        }
    }

    void OnCollisionEnter()
    {
        // Increment collision count
        numCollisions += 1;
    }

    void OnCollisionExit()
    {
        // Decrement collision count
        numCollisions -= 1;
    }

    void Update()
    {
        if (numCollisions == 0) {
            // Increase OOB timer when player is in the air
            oobTimer += Time.deltaTime;
        } else {
            // Reset OOB timer on ground
            oobTimer = 0.0f;
        }
        if (oobTimer >= outOfBoundsTime) {
            // Restart scene when OOB timer is large enough
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    void FixedUpdate()
    {
        // Determine ground normal using raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Physics.gravity.normalized, out hit, 0.75f)) {
            isOnGround = true;
            groundNormal = hit.normal;
        } else {
            isOnGround = false;
        }
        // Modify drag
        if (isOnGround) {
            if (Vector3.Dot(Physics.gravity.normalized, groundNormal) < -0.998f) {
                // Gravity is in line with normal, try to stop.
                rb.drag = dragStop;
                rb.angularDrag = angularDragStop;
            } else {
                // Use ground drag
                rb.drag = dragGround;
                rb.angularDrag = angularDragGround;
            }
        } else {
            // Use air drag
            rb.drag = dragAir;
            rb.angularDrag = angularDragAir;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup")) {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }
}
