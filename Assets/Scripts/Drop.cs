using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drop : MonoBehaviour
{
    private Collider2D collider;
    private PlayerMoveAndy playerScript;

    private bool playerIsOnPlatform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<Collider2D>();
        playerScript = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerMoveAndy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Drop
    public void DropFunction(InputAction.CallbackContext context)
    {
        if (context.performed && playerScript.isGrounded && playerIsOnPlatform && collider.enabled)
        {
            //Coroutine dropping
            StartCoroutine(DisablePlayerCollider(0.25f));

        }
    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        collider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        collider.enabled = true;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            playerIsOnPlatform = false;
        }
    }
}
