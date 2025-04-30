using UnityEngine;

public class ArmMovement : MonoBehaviour
{
    public Transform handTip; // Child at the end of the arm
    public Transform dog;
    public RopeConstraint rope;

    void Start()
    {
        if (handTip == null || dog == null || rope == null)
        {
            Debug.LogError("Missing references on ArmMovement script!");
            enabled = false;
            return;
        }

        // Set up rope connection
        Rigidbody2D handRb = handTip.GetComponent<Rigidbody2D>();
        Rigidbody2D dogRb = dog.GetComponent<Rigidbody2D>();

        if (handRb == null || dogRb == null)
        {
            Debug.LogError("Both HandTip and Dog need Rigidbody2D components!");
            enabled = false;
            return;
        }

        rope.Man = handRb;
        rope.Dog = dogRb;

        // Automatically calculate max rope length from initial pose
        //rope.maxRopeLength = Vector2.Distance(handTip.position, dog.position);
    }

    void Update()
    {
        // Rotate this (the arm GameObject) to point toward the dog
        Vector2 direction = dog.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}