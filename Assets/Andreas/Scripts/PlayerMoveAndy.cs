using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMoveAndy : MonoBehaviour
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

    // Jump cooldown
    public float jumpCooldown = 0.2f;
    private float jumpCooldownTimer = 0f;

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

    // Animator
    public Animator Ani;

    //Audio
    private AudioManager audioManager;

    private PlayerInput playerInput;


    public Rigidbody2D otherPlayerRB;
    public float leashLength = 3f;      // Default leash length
    public float pullForce = 50f;       // Force applied when pulling

    private static float currentLeashLength; // Static variable to store the current leash length


    void Awake()
    {
        // Ensure this static variable persists across scene loads if needed
        if (currentLeashLength == 0f)
        {
            currentLeashLength = leashLength; // Initialize with the default
        }
        leashLength = currentLeashLength;

        // --- NEW: Get PlayerInput Component ---
        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = true;
    }

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        playerRB.linearDamping = 0f; // Always no drag

        audioManager = AudioManager.Instance;
    }

    void Update()
    {
        GroundCheck();
        Gravity();
        WallSlide();
        ProcessWallJump();

        // Decrease jump cooldown timer
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        if (horizontalMove == 0f && isGrounded && !isWallSliding && !isWallJumping && Mathf.Abs(playerRB.linearVelocity.y) < 0.1f)
        {
            Ani.SetBool("Ideal", true);
            Ani.SetBool("Run", false);
            Ani.SetBool("Jump", false);
        }
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

        if (otherPlayerRB != null)
        {
            float currentDistance = Vector2.Distance(playerRB.position, otherPlayerRB.position);

            if (currentDistance > leashLength)
            {
                // Calculate direction from this player to the other
                Vector2 pullDirection = (otherPlayerRB.position - playerRB.position).normalized;

                // Apply a force to pull this player towards the other.
                // The force magnitude increases with how much the leash is stretched.
                float forceMagnitude = (currentDistance - leashLength) * pullForce;
                playerRB.AddForce(pullDirection * forceMagnitude, ForceMode2D.Force);
            }
        }
    }

    // Input: Move
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMove = context.ReadValue<Vector2>().x;

        if (Mathf.Abs(horizontalMove) > 0.1f && isGrounded)
        {
            audioManager.PlaySoundEffects(audioManager.walkSound); //Play walk sound
        }

        Ani.SetBool("Ideal", false);
        Ani.SetBool("Run", true);
        Ani.SetBool("Jump", false);
    }

    // Input: Jump
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpCooldownTimer > 0f)
            return; // Block jump if still in cooldown

        if (isWallSliding)
        {
            if (context.performed)
            {
                isWallJumping = true;
                playerRB.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
                wallJumpTimer = 0f;
                MoveJuice.Play();

                audioManager.PlaySoundEffects(audioManager.jumpSound);
                jumpCooldownTimer = jumpCooldown;

                // Flip forcefully
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
                audioManager.PlaySoundEffects(audioManager.jumpSound);
                jumpCooldownTimer = jumpCooldown;
            }
            else if (context.canceled)
            {
                playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.y * 0.5f);
                MoveJuice.Play();
            }
        }

        Ani.SetBool("Ideal", false);
        Ani.SetBool("Run", false);
        Ani.SetBool("Jump", true);
    }

    public void Drop(InputAction.CallbackContext context)
    {

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
        bool wasGrounded = isGrounded; // Save previous grounded state

        // Use OverlapBox to get the Collider2D of the ground object
        Collider2D groundCollider = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);

        if (groundCollider != null) // Check if something is actually detected as ground
        {
            isGrounded = true;

            // Check if the collided object is tagged as a "WheelPlatform" or "SeesawPlatform"
            if (groundCollider.CompareTag("WheelPlatform") || groundCollider.CompareTag("SeesawPlatform"))
            {

                jumpsRemaining = 1;
            }
            else
            {

                jumpsRemaining = maxJumps;
            }
            if (!wasGrounded)
            {
                audioManager.PlaySoundEffects(audioManager.landSound); // Play landing sound
            }
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

    public bool IsFacingRight()
    {
        return isFacingRight;
    }

    public void ChangeDifficulty(int difficulty)
    {
        if (difficulty == 0) // Easy
        {
            currentLeashLength = 5f;
        }
        else if (difficulty == 1) // Medium
        {
            currentLeashLength = 3f;
        }
        else if (difficulty == 2) // Hard
        {
            currentLeashLength = 1.5f;
        }
        leashLength = currentLeashLength; // Apply the new leash length
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}