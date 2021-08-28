using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    [SerializeField]
    private Transform transformToFollow;

    [SerializeField]
    private bool followCursor = false;

    private Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(0, 0, -10);
    }

    // Update is called once per frame
    void Update()
    {
        if (followCursor)
        {
            // transform.position = Input.mousePosition + offset;
        } else
        {
            transform.position = transformToFollow.position + offset;
        }
    }
}
