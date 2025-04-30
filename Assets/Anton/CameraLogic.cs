using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraLogic : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    [Header("Zoom Settings")]
    public float minZoom = 5f;
    public float maxZoom = 10f;
    public float zoomLimiter = 10f; // Higher means less zoom out
    public float zoomSmoothTime = 0.2f;

    [Header("Position Smoothing")]
    public float positionSmoothTime = 0.2f;

    private Vector3 velocity;
    private Camera cam;
    private float currentZoomVelocity;
    
    [Header("Dynamic Lookahead Settings")]
    public float idleYOffset = 1f;
    public float jumpYOffset = 3f;
    public float fallYOffset = -2f;
    public float verticalLookaheadSpeed = 2f;

    private float currentYOffset = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        MoveCamera();
        ZoomCamera();
    }

    void MoveCamera()
    {
        Vector3 midpoint = (player1.position + player2.position) / 2f;

        // Get average Y velocity of players
        float velocityY1 = GetVelocityY(player1);
        float velocityY2 = GetVelocityY(player2);
        float avgVelocityY = (velocityY1 + velocityY2) / 2f;

        // If falling, shift down. Otherwise, return to idle offset.
        float targetYOffset = avgVelocityY < -0.1f ? fallYOffset : idleYOffset;

        // Smooth interpolation
        currentYOffset = Mathf.Lerp(currentYOffset, targetYOffset, Time.deltaTime * verticalLookaheadSpeed);

        // Apply Y offset and keep Z
        midpoint.y += currentYOffset;
        midpoint.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, midpoint, ref velocity, positionSmoothTime);
    }

// Helper to get Rigidbody2D Y velocity
    float GetVelocityY(Transform player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        return rb ? rb.linearVelocity.y : 0f;
    }

    void ZoomCamera()
    {
        float distance = Vector2.Distance(player1.position, player2.position);
        float desiredZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, desiredZoom, ref currentZoomVelocity, zoomSmoothTime);
    }
}