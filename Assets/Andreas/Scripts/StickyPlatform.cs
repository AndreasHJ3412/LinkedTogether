using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    public float speedModifier = 0.5f;
    public float jumpModifier = 0.5f;

    private float originalSpeed;
    private float originalJumpForce;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMoveAndy player = collision.gameObject.GetComponent<PlayerMoveAndy>();

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
            PlayerMoveAndy player = collision.gameObject.GetComponent<PlayerMoveAndy>();

            player.moveSpeed = originalSpeed;
            player.jumpPower = originalJumpForce;
        }
    }
}
