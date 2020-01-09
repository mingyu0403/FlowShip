using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloat : MonoBehaviour
{
    public float airDrag = 1;
    public float waterDrag = 10;
    public Transform[] floatPoints;

    Rigidbody rigidbody;
    Waves waves;

    float waterLine;
    Vector3[] waterLinePoints;

    Vector3 centerOffset;

    Vector3 center { get { return transform.position + centerOffset; } }

    void Awake()
    {
        waves = FindObjectOfType<Waves>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;

        waterLinePoints = new Vector3[floatPoints.Length];
        for(int i=0; i<floatPoints.Length; i++)
        {
            waterLinePoints[i] = floatPoints[i].position;
        }
        centerOffset = PhysicsHelper.GetCenter(waterLinePoints) - transform.position;
    }

    void Update()
    {
        float newWaterLine = 0f;
        bool pointUnderWater = false;

        for(int i=0; i<floatPoints.Length; i++)
        {

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (floatPoints == null)
            return;

        for(int i=0; i<floatPoints.Length; i++)
        {
            if (floatPoints[i] == null)
                continue;

            if(waves != null)
            {
                // draw cube
                Gizmos.color = Color.red;
                Gizmos.DrawCube(waterLinePoints[i], Vector3.one * 0.3f);
            }

            // draw shpere
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(floatPoints[i].position, 0.1f);
        }

        // draw center
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(center.x, waterLine, center.z), Vector3.one * 1f);
        }
    }
}
