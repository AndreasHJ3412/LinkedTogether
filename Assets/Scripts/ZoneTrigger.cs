using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public enum ZoneType { Forest, City, Mountain, Space }
    public ZoneType zoneType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.ChangeZone(zoneType);
        }
    }
}

