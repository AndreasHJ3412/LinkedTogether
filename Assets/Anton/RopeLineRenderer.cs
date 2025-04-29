using UnityEngine;
using System.Collections.Generic;

public class RopeLineRenderer : MonoBehaviour
{
    public Transform Man;
    public Transform Dog;
    public LineRenderer lineRenderer;
    public LayerMask obstructionLayer; // Layer containing buildings/obstacles
    public float checkFrequency = 0.5f; // How often to check for obstructions (adjust as needed)
    public float obstructionCheckRadius = 0.2f; // Radius to check around the direct line

    private float lastCheckTime;

    void LateUpdate()
    {
        if (Man == null || Dog == null || lineRenderer == null) return;

        if (Time.time - lastCheckTime >= checkFrequency)
        {
            lastCheckTime = Time.time;
            UpdateRopePoints();
        }
        else if (lineRenderer.positionCount > 0 && (Vector2)lineRenderer.GetPosition(0) != (Vector2)Man.position || (Vector2)lineRenderer.GetPosition(lineRenderer.positionCount - 1) != (Vector2)Dog.position)
        {
            // Ensure endpoints always follow players even if not checking every frame
            UpdateRopeEndpointsOnly();
        }
    }

    void UpdateRopeEndpointsOnly()
    {
        int pointCount = lineRenderer.positionCount;
        if (pointCount >= 2)
        {
            Vector3[] positions = new Vector3[pointCount];
            lineRenderer.GetPositions(positions);
            positions[0] = Man.position;
            positions[pointCount - 1] = Dog.position;
            lineRenderer.SetPositions(positions);
        }
        else if (pointCount == 1)
        {
            lineRenderer.SetPosition(0, Man.position);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(1, Dog.position);
        }
        else if (pointCount == 0)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, Man.position);
            lineRenderer.SetPosition(1, Dog.position);
        }
    }

    void UpdateRopePoints()
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(Man.position);

        Vector2 direction = Dog.position - Man.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.CircleCast(Man.position, obstructionCheckRadius, direction.normalized, distance, obstructionLayer);

        if (hit.collider != null)
        {
            // Obstruction detected, find a point to go around it (simplified)
            Vector2 hitNormal = hit.normal;
            Vector2 tangent = Vector2.Perpendicular(hitNormal).normalized;

            // Try going around in both directions
            Vector2 offset1 = hit.point + tangent * obstructionCheckRadius * 2f;
            Vector2 offset2 = hit.point - tangent * obstructionCheckRadius * 2f;

            // Add the point that's further along the direction to the target
            if (Vector2.Dot((offset1 - (Vector2)Man.position).normalized, direction.normalized) >
                Vector2.Dot((offset2 - (Vector2)Man.position).normalized, direction.normalized))
            {
                points.Add(offset1);
            }
            else
            {
                points.Add(offset2);
            }
            points.Add(Dog.position);
        }
        else
        {
            // No obstruction, just two points
            points.Add(Dog.position);
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
