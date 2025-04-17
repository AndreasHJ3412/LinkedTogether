using System;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

public class PassThroughPlatform : MonoBehaviour
{
    private Collider2D platformCollider;
    private PlayerMovement playerScript;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        platformCollider = GetComponent<Collider2D>();

        playerScript = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerMovement>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.isGrounded() && Input.GetAxisRaw("Vertical") < 0)
        {
            platformCollider.enabled = false;
            StartCoroutine(EnableCollider());
        }
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);
        platformCollider.enabled = true;
    }
}


