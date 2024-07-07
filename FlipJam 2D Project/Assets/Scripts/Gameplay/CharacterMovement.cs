using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 5;
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    private bool canDoubleJump = true;
    private bool isDashing = false;
    private float dashTimeLeft;

    Rigidbody2D rig2D;
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    bool isGrounded = false;

    public float hangTime = .3f;
    float hangCounter = 0f;
    public float jumpBufferLength = .3f;
    float jumpBufferCounter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rig2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            Vector2 movement = new Vector2(horizontal * speed, rig2D.velocity.y);
            rig2D.velocity = movement;

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
                rig2D.velocity = new Vector2(rig2D.velocity.x, jumpForce);
                jumpBufferCounter = 0f;
                if (!isGrounded)
                {
                    canDoubleJump = false; // Allow double jump after the initial jump
                }
            }
            else if (Input.GetButtonDown("Jump") && !isGrounded && canDoubleJump)
            {
                rig2D.velocity = new Vector2(rig2D.velocity.x, jumpForce);
                canDoubleJump = false; // Disable double jump after using it
            }

            if (Input.GetButtonUp("Jump") && rig2D.velocity.y > 0f)
            {
                Vector2 slowFall = new Vector2(rig2D.velocity.x, rig2D.velocity.y * .5f);
                rig2D.velocity = slowFall;
            }

            if (Input.GetButtonDown("Dash"))
            {
                isDashing = true;
                dashTimeLeft = dashTime;
                rig2D.velocity = new Vector2(horizontal * dashSpeed, rig2D.velocity.y);
            }
        }
        else
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
        }
    }
}
