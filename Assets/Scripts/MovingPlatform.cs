using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    public float moveSpeed = 2f;

    private Vector3 nextPosition;
    
    void Start()
    {
        nextPosition = endPoint.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == startPoint.position) ? endPoint.position : startPoint.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null;
        }
    }
}
