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
    public float outOfBoundsTime = 2.5f;

    private Rigidbody rb;

    private int count = 0;
    private int numCollisions = 0;
    private float oobTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetCountText();
        winTextObject.SetActive(false);
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12) {
            winTextObject.SetActive(true);
        }
    }

    void OnCollisionEnter()
    {
        numCollisions += 1;
    }

    void OnCollisionExit()
    {
        numCollisions -= 1;
    }

    void Update()
    {
        if (numCollisions == 0) {
            oobTimer += Time.deltaTime;
        } else {
            oobTimer = 0.0f;
        }
        if (oobTimer >= outOfBoundsTime) {
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
