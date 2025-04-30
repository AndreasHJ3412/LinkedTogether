using UnityEngine;

public class ArmMovement : MonoBehaviour
{
    public Transform handTip;
    public Transform dog;
    public RopeConstraint rope;
    public PlayerMoveAndy player; // Reference to check facing direction
    private SpriteRenderer armRenderer; // To control arm's sorting

    void Start()
    {
        if (handTip == null || dog == null || rope == null || player == null)
        {
            Debug.LogError("Missing references on ArmMovement script!");
            enabled = false;
            return;
        }

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

        // Get the arm's SpriteRenderer to set sorting order
        armRenderer = GetComponent<SpriteRenderer>();
        if (armRenderer == null)
        {
            Debug.LogError("Arm does not have a SpriteRenderer!");
            enabled = false;
        }
    }

    void Update()
    {
        Vector2 toDog = dog.position - handTip.position;

        if (toDog.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(toDog.y, toDog.x) * Mathf.Rad2Deg;

        // If the dog is below the player, restrict extreme angles
        if (toDog.y < 0.2f)
            angle = Mathf.Clamp(angle, -60f, 60f);

        // Flip arm independently based on player facing direction
        if (player.IsFacingRight())
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.localScale = new Vector3(1, 1, 1); // Arm scale doesn't flip with player
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, -angle); // Mirror the arm's rotation
            transform.localScale = new Vector3(1, 1, 1); // Keep arm's scale unaffected by player flip
        }

        // Ensure the arm appears behind the player in the sorting order
        if (armRenderer != null)
        {
            armRenderer.sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder - 1;
        }
    }
}
