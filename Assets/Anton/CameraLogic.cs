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
        midpoint.z = transform.position.z; // Maintain camera's z position
        transform.position = Vector3.SmoothDamp(transform.position, midpoint, ref velocity, positionSmoothTime);
    }

    void ZoomCamera()
    {
        float distance = Vector2.Distance(player1.position, player2.position);
        float desiredZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, desiredZoom, ref currentZoomVelocity, zoomSmoothTime);
    }
}