using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHelper : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public static Vector3 GetCenter(Vector3[] points)
    {
        Vector3 center = Vector3.zero;
        for(int i=0; i<points.Length; i++)
        {
            center += points[i] / points.Length;
        }
        return center;
    }
}
