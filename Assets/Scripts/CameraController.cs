using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerReference;
    private GameObject parent;
    // private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        // offset = transform.position - playerReference.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        parent.transform.position = playerReference.transform.position;
    }
}
