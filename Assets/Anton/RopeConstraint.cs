using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeConstraint : MonoBehaviour
{
    [Header("Rope Physics")]
    public Rigidbody2D playerA;
    public Rigidbody2D playerB;
    public float maxRopeLength = 5f;
    public float ropeStiffness = 100f;
    public float ropeDamping = 5f;

    [Header("Rope Visual")] 
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public int maxBendPoints = 10;
    public float maxSlackCurveAmount = 0.5f;
    public int smoothingSegments = 20;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        if (playerA == null || playerB == null) return;

        Vector2 ropeVector = playerB.position - playerA.position;
        float distance = ropeVector.magnitude;
        Vector2 ropeDir = ropeVector.normalized;

        if (distance > maxRopeLength)
        {
            float overshoot = distance - maxRopeLength;

            Vector2 relativeVelocity = playerB.linearVelocity - playerA.linearVelocity;
            float relVelAlongRope = Vector2.Dot(relativeVelocity, ropeDir);

            if (relVelAlongRope > 0f)
            {
                Vector2 correction = ropeDir * relVelAlongRope;
                playerA.linearVelocity += correction * 0.5f;
                playerB.linearVelocity -= correction * 0.5f;
            }

            Vector2 pullForce = ropeDir * overshoot * ropeStiffness;
            playerA.AddForce(pullForce * 0.5f, ForceMode2D.Force);
            playerB.AddForce(-pullForce * 0.5f, ForceMode2D.Force);

            Vector2 dampingForce = ropeDir * Vector2.Dot(relativeVelocity, ropeDir) * ropeDamping;
            playerA.AddForce(-dampingForce * 0.5f, ForceMode2D.Force);
            playerB.AddForce(dampingForce * 0.5f, ForceMode2D.Force);
        }

        UpdateRopeVisual();
    }

    void UpdateRopeVisual()
    {
        if (line == null || playerA == null || playerB == null) return;

        // Step 1: Generate raw rope points around obstacles
        List<Vector3> ropePoints = new List<Vector3>();
        Vector2 start = playerA.position;
        Vector2 end = playerB.position;
        ropePoints.Add(start);

        Vector2 from = start;
        for (int i = 0; i < maxBendPoints; i++)
        {
            Vector2 to = end;
            RaycastHit2D hit = Physics2D.Raycast(from, (to - from).normalized, Vector2.Distance(from, to), groundLayer);
            if (hit.collider != null)
            {
                Vector2 bendPoint = hit.point + hit.normal * groundCheckRadius;
                ropePoints.Add(bendPoint);
                from = bendPoint;
            }
            else break;
        }
        ropePoints.Add(end);

        // Step 2: Calculate slack-based curve amount
        float totalDistance = Vector2.Distance(start, end);
        float slackMultiplier = Mathf.Clamp01(1f - (totalDistance / maxRopeLength));
        float slackCurveAmount = maxSlackCurveAmount * slackMultiplier;

        // Step 3: Apply Bezier smoothing
        List<Vector3> smoothPoints = new List<Vector3>();
        if (ropePoints.Count >= 3)
        {
            for (int i = 0; i <= smoothingSegments; i++)
            {
                float t = i / (float)smoothingSegments;
                Vector3 pt = DeCasteljau(ropePoints, t);
                // Optional: push down for slack curve when no obstacles
                if (slackCurveAmount > 0f && ropePoints.Count == 2)
                {
                    pt.y -= Mathf.Sin(t * Mathf.PI) * slackCurveAmount;
                }
                smoothPoints.Add(pt);
            }
        }
        else
        {
            // Not enough bends, just apply slack curve on simple line
            for (int i = 0; i <= smoothingSegments; i++)
            {
                float t = i / (float)smoothingSegments;
                Vector3 pt = Vector3.Lerp(start, end, t);
                pt.y -= Mathf.Sin(t * Mathf.PI) * slackCurveAmount;
                smoothPoints.Add(pt);
            }
        }

        // Step 4: Render
        line.positionCount = smoothPoints.Count;
        line.SetPositions(smoothPoints.ToArray());
    }

    // De Casteljau's algorithm for Bezier curves
    Vector3 DeCasteljau(List<Vector3> pts, float t)
    {
        Vector3[] temp = pts.ToArray();
        int n = temp.Length;
        for (int k = 1; k < n; k++)
        {
            for (int i = 0; i < n - k; i++)
            {
                temp[i] = Vector3.Lerp(temp[i], temp[i + 1], t);
            }
        }
        return temp[0];
    }

    // Public method to adjust line width at runtime
    public void SetLineWidth(float width)
    {
        if (line != null)
        {
            line.startWidth = width;
            line.endWidth = width;
        }
    }
}
