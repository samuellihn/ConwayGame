using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCam;
    public float scrollSpeed = 0.125f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mainCam.orthographicSize -= Input.mouseScrollDelta.y;
        if (Input.GetKey("w"))
        {
            mainCam.transform.position += new Vector3(0f, 1f * scrollSpeed, 0f);
        }
        if (Input.GetKey("a"))
        {
            mainCam.transform.position += new Vector3(-1f * scrollSpeed, 0f, 0f);
        }
        if (Input.GetKey("s"))
        {
            mainCam.transform.position += new Vector3(0f, -1f * scrollSpeed, 0f);
        }
        if (Input.GetKey("d"))
        {
            mainCam.transform.position += new Vector3(1f * scrollSpeed, 0f, 0f);
        }
    }
}
