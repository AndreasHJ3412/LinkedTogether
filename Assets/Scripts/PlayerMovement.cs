using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Asign Variables 
    public float speed;

    public float jumpForce;

    public float move;

    public Rigidbody2D playerBody;

    public Vector2 castBoxSize;
    public float castDistance;
    public LayerMask groundLayer;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        move = Input.GetAxis("Horizontal");
        playerBody.linearVelocity = new Vector2(speed * move, playerBody.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            playerBody.AddForce(new Vector2(playerBody.linearVelocity.x, jumpForce));
        }
    }

    public bool isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, castBoxSize, 0, -transform.up, castDistance, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position-transform.up * castDistance, castBoxSize);
    }
}
