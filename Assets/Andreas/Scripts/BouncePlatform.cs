using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    public float bounceForce = 15f; 
    public PlayerMoveAndy player;   

    public Sprite defaultSprite;    
    public Sprite bounceSprite;    
    public float resetDelay = 0.1f; 

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            // Reset vertical velocity for consistent bounce
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
           
            // Give upward force
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

            spriteRenderer.sprite = bounceSprite;
           
            // Schedule sprite reset after delay
            Invoke(nameof(ResetSprite), resetDelay);
        }
    }

    void ResetSprite()
    {
        spriteRenderer.sprite = defaultSprite;
    }
}
