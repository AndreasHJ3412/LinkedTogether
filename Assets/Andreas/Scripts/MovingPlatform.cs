using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovingPlatform : MonoBehaviour
{
    public Transform platformStart;
    public Transform platformEnd;
    public float speed = 2f;

    private Vector3 platformTarget;
    private float switchDistance = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
            platformTarget = platformEnd.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, platformTarget, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, platformTarget) < switchDistance)
        {
            if (platformTarget == platformStart.position)
            {
                platformTarget = platformEnd.position;
            }
            else
            {
                platformTarget = platformStart.position;
            }
        }
    }
}
