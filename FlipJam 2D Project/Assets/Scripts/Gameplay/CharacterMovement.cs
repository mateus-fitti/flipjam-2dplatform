using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 5;
    private float speed;
    [SerializeField] private float defaultJumpForce = 5;
    private float jumpForce;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.2f;
    private bool canDoubleJump = true;
    private bool isDashing = false;
    private float dashTimeLeft;

    Rigidbody2D rig2D;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    bool isGrounded = false;

    [SerializeField] private float hangTime = .3f;
    float hangCounter = 0f;
    [SerializeField] private float jumpBufferLength = .3f;
    float jumpBufferCounter = 0f;
    
    [SerializeField] private float weightValue = .5f;

    [SerializeField] private float gravityScale = 10;
    [SerializeField] private float fallingGravityScale = 40;

    public Boolean canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rig2D = GetComponent<Rigidbody2D>();

        DefaultMovement();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
        MovementHorizontal();
        Jump();
        }
    }

    void MovementHorizontal()
    {
        if (!isDashing)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");

            Vector2 movement = new Vector2(horizontal * speed, rig2D.velocity.y);
            rig2D.velocity = movement;

            if (Input.GetButtonDown("Dash"))
            {
                isDashing = true;
                dashTimeLeft = dashTime;
                rig2D.velocity = new Vector2(horizontal * dashSpeed, rig2D.velocity.y);
            }
        } else {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
        }
    }

    void Jump()
    {
        if (!isDashing)
        {
            isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, .1f, groundLayer);

            if (isGrounded)
            {
                hangCounter = hangTime;
                canDoubleJump = true; // Reset the double jump when grounded
            }
            else
            {
                hangCounter -= Time.deltaTime;
            }

            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferLength;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            if (jumpBufferCounter > 0f && (isGrounded || hangCounter > 0f))
            {
                rig2D.velocity = new Vector2(rig2D.velocity.y, jumpForce);
                jumpBufferCounter = 0f;
            }
            else if (Input.GetButtonDown("Jump") && !isGrounded && canDoubleJump)
            {
                rig2D.velocity = new Vector2(rig2D.velocity.y, jumpForce);
                canDoubleJump = false; // Disable double jump after using it
            }

            if (Input.GetButtonUp("Jump") && rig2D.velocity.y > 0f)
            {
                Vector2 slowFall = new Vector2(rig2D.velocity.x, rig2D.velocity.y * .5f);
                rig2D.velocity = slowFall;
            }

            if(rig2D.velocity.y >= 0)
            {
                rig2D.gravityScale = gravityScale;
            }else if(rig2D.velocity.y < 0)
            {
                rig2D.gravityScale = fallingGravityScale;
            }


        }
    }

    public void HeavyMovement()
    {
        speed *= weightValue;
        jumpForce *= weightValue;
    }

    public void DefaultMovement()
    {
        speed = defaultSpeed;
        jumpForce = defaultJumpForce;
    }
}
