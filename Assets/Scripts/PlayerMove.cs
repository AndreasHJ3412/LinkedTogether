using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private bool isFacingRight = true;
    public ParticleSystem MoveJuice;
    private BoxCollider2D playerCollider;
    private bool isOnIce = false;
    
    //Movement
    public float moveSpeed = 5f;
    private float horizontalMove;
    
    //Jumping
    public float jumpPower = 10f;
    private int maxJumps = 2;
    private int jumpsRemaining;
    
    //GroundCheck
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public bool isGrounded;
    private bool isOnPlatform;
    
    //WallCheck
    public Transform WallCheckPos;
    public Vector2 WallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask WallLayer;
    
    //Gravity 
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;
    
    //Wall Movement
    public float wallSlideSpeed = 2f;
    private bool isWallSliding;

    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpTime = 0.5f;
    private float wallJumpTimer;
    public Vector2 WallJumpPower = new Vector2(5f, 10f);
    
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            playerRB = GetComponent<Rigidbody2D>();
            playerCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Gravity();
        WallSlide();
        ProcessWallJump();

        /*
        if (!isWallJumping)
        {
            // Normal movement
            playerRB.velocity = new Vector2(horizontalMove * moveSpeed, playerRB.velocity.y);
            
            Flip();
        }
        */
    }
    
    void FixedUpdate()
    {
        float targetSpeed = horizontalMove * moveSpeed;

        if (isOnIce)
        {
            // On ice: Use AddForce with low drag
            playerRB.AddForce(new Vector2(targetSpeed - playerRB.linearVelocity.x, 0), ForceMode2D.Force);
        }
        else if (!isWallJumping)
        {
            // Normal direct velocity control
            playerRB.linearVelocity = new Vector2(targetSpeed, playerRB.linearVelocity.y);
            Flip();
        }
    }


    
    
    //Move 
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMove = context.ReadValue<Vector2>().x;
    }

    //Flip
    private void Flip()
    {
        if (isFacingRight && horizontalMove < 0 || !isFacingRight && horizontalMove > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            if (playerRB.linearVelocity.y == 0)
            {
                MoveJuice.Play();
            }
        }  
    }
    
    
    //Checks
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize,0f, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(WallCheckPos.position, WallCheckSize, 0f, WallLayer);
    }
    
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(WallCheckPos.position, WallCheckSize);
    }

    
    
    //Gravity
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
    

    //Jump
    public void Jump(InputAction.CallbackContext context)
    {
        //wall-jump
        if (isWallSliding)
        {
            isWallJumping = true;
            playerRB.linearVelocity = new Vector2(wallJumpDirection * WallJumpPower.x, WallJumpPower.y); //makes player jump away from the wall
            wallJumpTimer = 0f;
            MoveJuice.Play();
        
            //force-flip
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); //walljump will last 0.5s and we can jump again after 0.6s
        }
        //normal-jump
        else if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                //Jump-button help down = full jump height
                playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                MoveJuice.Play();
            }
            else if (context.canceled)
            {
                // Light tab on jump-button = half jump height 
                playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.y * 0.5f);
                jumpsRemaining--;
                MoveJuice.Play();
            }
        }
    }

    
    //Drop
    /*
    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && isOnPlatform && playerCollider.enabled)
        {
            //Coroutine dropping
            StartCoroutine(DisablePlayerCollider(0.25f));

        }
    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled = true;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }
    */
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            isOnIce = true;
            playerRB.linearDamping = 1.5f; // Or experiment: 2–5 for more friction control
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            isOnIce = false;
            playerRB.linearDamping = 0; // Or experiment: 2–5 for more friction control
        }
    }
    

    //Wall-Stuff
    private void WallSlide()
    {
        if (!isGrounded & WallCheck() & horizontalMove != 0)
        {
            isWallSliding = true;
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, Mathf.Max(playerRB.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }
    
    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;
            
            CancelInvoke(nameof(CancelWallJump)); //as soon as we wallslide we can jump again. 
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
}
