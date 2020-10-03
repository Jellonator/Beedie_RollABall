using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerReference;
    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        parent.transform.position = playerReference.transform.position;
    }
}
