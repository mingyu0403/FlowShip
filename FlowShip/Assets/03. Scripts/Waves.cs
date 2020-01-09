using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메쉬를 생성하고, 버텍스 애니메이션을 이용하여 Wave를 제어하는 클래스입니다.
/// </summary>
public class Waves : MonoBehaviour
{
    [Header("- 메쉬 크기")]
    public int DimensionX = 10;
    public int DimensionZ = 10;
    [Header("- 발생될 파도 구조체 배열")]
    public Octave[] Octaves;
    [Header("- UV 스케일")]
    public float UVScaleX;
    public float UVScaleZ;

    MeshFilter meshFilter; 
    Mesh mesh;


    void Start()
    {
        mesh = new Mesh();
        mesh.name = gameObject.name;
        
        mesh.vertices = GenerateVerts();
        mesh.triangles = GenerateTries();
        mesh.uv = GenerateUvs();

        // 다시 계산하기
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // 포지션을 이동시켜서 메쉬가 적절한 위치에 오도록 함. 
        // (Vertex 생성 시에 바로 대입하려 했으나, 그냥 오브젝트의 위치를 옮기는 것이 낫다고 판단했음.)
        transform.position = new Vector3(transform.position.x - DimensionX / 2, 0, -10f );
    }


    /// <summary>
    /// 버텍스 생성하기
    /// </summary>
    /// <returns>생성한 버텍스</returns>
    private Vector3[] GenerateVerts()
    {
        var verts = new Vector3[(DimensionX + 1) * (DimensionZ + 1)];

        for(int x=0; x<=DimensionX; x++)
        {
            for(int z=0; z<=DimensionZ; z++)
            {
                Debug.Log(verts.Length);

                verts[index(x, z)] = new Vector3(x, 0, z);
            }
        }
        return verts;
    }

    private int index(int x, int z)
    {
        return x * (DimensionZ + 1) + z;
    }

    /// <summary>
    /// 생성된 버텍스를 이용해 삼각형으로 이어주기
    /// </summary>
    /// <returns>생성한 Tries</returns>
    private int[] GenerateTries()
    {
        var tries = new int[mesh.vertices.Length * 6];

        for(int x=0; x<DimensionX; x++)
        {
            for(int z=0; z<DimensionZ; z++)
            {
                // 삼각형 그리기
                tries[index(x, z) * 6 + 0] = index(x, z);
                tries[index(x, z) * 6 + 1] = index(x+1, z+1);
                tries[index(x, z) * 6 + 2] = index(x+1, z);
                tries[index(x, z) * 6 + 3] = index(x, z);
                tries[index(x, z) * 6 + 4] = index(x, z+1);
                tries[index(x, z) * 6 + 5] = index(x+1, z+1);
            }
        }
        return tries;
    }

    /// <summary>
    /// UV 계산하기
    /// </summary>
    /// <returns>계산된 UV</returns>
    private Vector2[] GenerateUvs()
    {
        var uvs = new Vector2[mesh.vertices.Length];

        for(int x=0; x<=DimensionX; x++)
        {
            for(int z=0; z<=DimensionZ; z++)
            {
                var vec = new Vector2((x / UVScaleX) % 2, (z / UVScaleZ) % 2);
                uvs[index(x, z)] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
            }
        }

        return uvs;
    }

    [Serializable]
    public struct Octave
    {
        public Vector2 speed;
        public Vector2 scale;
        public float height;
        public bool alternate;
    }

    void Update()
    {
        var verts = mesh.vertices;

        for (int x = 0; x <= DimensionX; x++)
        {
            for (int z = 0; z <= DimensionZ; z++)
            {
                float y = 0f;
                for (int o = 0; o < Octaves.Length; o++)
                {
                    if (Octaves[o].alternate)
                    {
                        //y += Mathf.Cos(Octaves[o].speed.magnitude * Time.time) * Octaves[o].height;
                        float per1 = Mathf.PerlinNoise((x * Octaves[o].scale.x) / DimensionX, (z * Octaves[o].scale.y) / DimensionZ) * Mathf.PI * 2f;
                        y += Mathf.Cos(per1 + Octaves[o].speed.magnitude * Time.time) * Octaves[o].height;
                    }
                    else
                    {
                        float NoiseX = (x * Octaves[o].scale.x + Time.time * Octaves[o].speed.x) / DimensionX;
                        float NoiseY = (z * Octaves[o].scale.y + Time.time * Octaves[o].speed.y) / DimensionZ;

                        // (불안정한 파도)
                        // PerlinNoise 함수가 0~1 범위이기 때문에 -0.5f를 해서 -0.5~0.5 범위가 되게 함.
                        // float per1 = Mathf.PerlinNoise(NoiseX, NoiseY) - 0.5f;

                        // (규칙적인 파도)
                        float per1 = Mathf.Sin(NoiseX)*0.5f; // (범위 교체) sin : -1~1 에서 -0.5~0.5
                        float per2 = Mathf.Sin(NoiseY)*0.5f; // (범위 교체) sin : -1~1 에서 -0.5~0.5

                        y += (per1 + per2) * Octaves[o].height;
                    }
                }
                verts[index(x, z)] = new Vector3(x, y, z);
            }
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();

    }
}
