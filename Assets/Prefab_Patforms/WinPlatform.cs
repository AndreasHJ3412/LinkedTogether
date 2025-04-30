using UnityEngine;

public class WinPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            FindAnyObjectByType<PlatformGenerator>().WinScreen.SetActive(true);
        }
    }
}
