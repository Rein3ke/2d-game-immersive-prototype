using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private float walkingSpeed = 3.0f;
    private float jumpHeight = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            rb.velocity += Vector2.left * walkingSpeed;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rb.velocity += Vector2.right * walkingSpeed;
        }
        if (Input.GetKeyDown(KeyCode.Space) && checkIsOnGround())
        {
            rb.velocity += Vector2.up * jumpHeight;
        }
    }

    private bool checkIsOnGround()
    {
        return true;
    }
}
