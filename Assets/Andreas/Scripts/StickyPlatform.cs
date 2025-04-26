using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    public float speedModifier = 0.5f;
    public float jumpModifier = 0.5f;

    private float originalSpeed;
    private float originalJumpForce;

    public PlayerMoveAndy player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            originalSpeed = player.moveSpeed;
            originalJumpForce = player.jumpPower;

            player.moveSpeed *= speedModifier;
            player.jumpPower *= jumpModifier;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.moveSpeed = originalSpeed;
            player.jumpPower = originalJumpForce;
        }
    }
}
