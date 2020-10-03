using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    /// The amount of time that the player may be OOB before restarting a level
    public float outOfBoundsTime = 2.5f;
    private Rigidbody rb;
    /// The number of collectibles that the player has collected
    private int count = 0;
    /// Track the number of walls that the player is touching
    private int numCollisions = 0;
    /// Timer used to track the amount of time that the player is considered OOB
    private float oobTimer = 0.0f;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup")) {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }
}
