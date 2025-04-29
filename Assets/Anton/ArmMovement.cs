using UnityEngine;

public class ArmMovement : MonoBehaviour
{
    public Transform shoulder;
    public Transform handTip;
    public Transform dog;
    public RopeConstraint rope; // Reference to the RopeConstraint script

    private float armLength;

    void Start()
    {
        if (shoulder == null || handTip == null || dog == null || rope == null)
        {
            Debug.LogError("One or more references are not assigned!");
            enabled = false;
            return;
        }
        armLength = Vector2.Distance(shoulder.position, handTip.localPosition); // Assuming initial local position is arm length

        // Ensure RopeConstraint is set up
        if (handTip.GetComponent<Rigidbody2D>() == null || dog.GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogError("HandTip or Dog needs a Rigidbody2D for the rope to work!");
            enabled = false;
            return;
        }
        rope.Man = handTip.GetComponent<Rigidbody2D>();
        rope.Dog = dog.GetComponent<Rigidbody2D>();
        rope.maxRopeLength = armLength; // Set initial rope length
    }

    void Update()
    {
        // Point the arm (shoulder) towards the dog
        Vector2 directionToDog = dog.position - shoulder.position;
        float angle = Mathf.Atan2(directionToDog.y, directionToDog.x) * Mathf.Rad2Deg;
        shoulder.rotation = Quaternion.Euler(0, 0, angle);

        // The RopeConstraint will now handle the visual and physical connection
        // You might not need to explicitly set handTip.position here anymore
    }
}