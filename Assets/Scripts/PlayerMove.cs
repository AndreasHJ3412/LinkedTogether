using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private bool isFacingRight = true;
    public ParticleSystem MoveJuice;
    private BoxCollider2D playerCollider;
    private bool isOnIce = false;

    // Movement
    public float moveSpeed = 5f;
    private float horizontalMove;

    // Jumping
    public float jumpPower = 10f;
    private int maxJumps = 2;
    private int jumpsRemaining;

    // Ground Check
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public bool isGrounded;
    private bool isOnPlatform;

    // Wall Check
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    // Gravity
    public float baseGravity = 2f;
    public float fallSpeedMultiplier = 2f;
    public float maxFallSpeed = 18f;

    // Wall Movement
    public float wallSlideSpeed = 2f;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpTime = 0.5f;
    private float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        playerRB.linearDamping = 0f; // Always no drag
    }

    void Update()
    {
        GroundCheck();
        Gravity();
        WallSlide();
        ProcessWallJump();
    }

    void FixedUpdate()
    {
        if (!isWallJumping)
        {
            float targetSpeed = horizontalMove * moveSpeed;
            float speedDifference = targetSpeed - playerRB.linearVelocity.x;

            float controlFactor = isOnIce ? 0.3f : 10f; // Less control on ice
            playerRB.AddForce(new Vector2(speedDifference * controlFactor, 0f), ForceMode2D.Force);

            Flip();
        }
    }

    // Input: Move
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMove = context.ReadValue<Vector2>().x;
    }

    // Input: Jump
    public void Jump(InputAction.CallbackContext context)
    {
        if (isWallSliding)
        {
            if (context.performed)
            {
                isWallJumping = true;
                playerRB.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
                wallJumpTimer = 0f;
                MoveJuice.Play();

                // Force flip
                if (transform.localScale.x != wallJumpDirection)
                {
                    isFacingRight = !isFacingRight;
                    Vector3 localScale = transform.localScale;
                    localScale.x *= -1f;
                    transform.localScale = localScale;
                }

                Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
            }
        }
        else if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                MoveJuice.Play();
            }
            else if (context.canceled)
            {
                playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.y * 0.5f);
                MoveJuice.Play();
            }
        }
    }

    // Flip the player based on move direction
    private void Flip()
    {
        if (isFacingRight && horizontalMove < 0 || !isFacingRight && horizontalMove > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            if (Mathf.Abs(playerRB.linearVelocity.y) < 0.01f)
            {
                MoveJuice.Play();
            }
        }
    }

    // Gravity adjustments
    private void Gravity()
    {
        if (playerRB.linearVelocity.y < 0)
        {
            playerRB.gravityScale = baseGravity * fallSpeedMultiplier;
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, Mathf.Max(playerRB.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            playerRB.gravityScale = baseGravity;
        }
    }

    // Ground check
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    // Wall check
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallLayer);
    }

    // Wall sliding
    private void WallSlide()
    {
        if (!isGrounded && WallCheck() && horizontalMove != 0)
        {
            isWallSliding = true;
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, Mathf.Max(playerRB.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    // Wall jump management
    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    // Ice detection
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            isOnIce = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            isOnIce = false;
        }
    }

    // Visual debug in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
