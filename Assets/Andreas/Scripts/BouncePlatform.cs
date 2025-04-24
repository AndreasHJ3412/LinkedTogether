using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    public float bounceForce = 15f;
    public PlayerMove player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            // Reset vertical speed so the bounce force is consistent
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // Apply bounce force
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
    }
}
