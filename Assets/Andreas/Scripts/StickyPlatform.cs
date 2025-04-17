using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    public float speedModifier = 0.5f;
    public float jumpModifier = 0.5f;

    private float originalSpeed;
    private float originalJumpForce;

    public PlayerMovement player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            originalSpeed = player.speed;
            originalJumpForce = player.jumpForce;

            player.speed *= speedModifier;
            player.jumpForce *= jumpModifier;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.speed = originalSpeed;
            player.jumpForce = originalJumpForce;
        }
    }
}
