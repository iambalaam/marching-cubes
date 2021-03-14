using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteAlways]
public class MeshGenerator : MonoBehaviour
{
    private static Vector3Int mapSize = new Vector3Int(50, 10, 50);

    Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = GetComponentInParent<MeshRenderer>().sharedMaterial;
        Debug.Log(GetComponentInParent<MeshRenderer>().sharedMaterial);

        GenerateMesh();
    }

    MarchingCubes.ScalarField field = (Vector3 v) => {
        float heightBias = (1 - v.y) * 0.12f;
        float noise = Noise.Sample(v / 10f);
        return heightBias + noise;
    };

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + ((Vector3)mapSize / 2f), mapSize);
    }

    private void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    Vector3[] tris = MarchingCubes.TriangulateCube(new Vector3(x, y, z), (v) => field(v + transform.position));
                    vertices.AddRange(tris);
                }
            }
        }

        int[] triangles = Enumerable.Range(0, vertices.Count).ToArray();
        Color[] colors = new Color[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            colors[i] = Color.Lerp(Color.blue, Color.red, (vertices[i].y / 10f));
        }


        mesh.vertices = vertices.ToArray();
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

}
