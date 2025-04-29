using UnityEngine;

public class WinPlatform : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        FindAnyObjectByType<PlatformGenerator>().WinScreen.SetActive(true);
    }
}
