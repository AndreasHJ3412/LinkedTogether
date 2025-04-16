using System;
using UnityEngine;

public class LeashController : MonoBehaviour
{
    private LineRenderer lr;
    public Transform Human;
    public Transform Dogi;

    public float fixedlength;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Vector3 direction = (Dogi.position - Human.position).normalized;
        Vector3 clamped = Human.position + direction * fixedlength;
        
        lr.SetPosition(0, Human.position);
        lr.SetPosition(1, clamped);
    }
}
