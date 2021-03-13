using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    private static Vector3Int mapSize = new Vector3Int(50, 10, 50);

    Mesh mesh;

    MarchingCubes.ScalarField field = (Vector3 v) => {
        // Adding nice edges to the map
        if (v.x == 0 || v.x == mapSize.x) return -0.01f;
        if (v.z == 0 || v.z == mapSize.z) return -0.01f;
        float heightBias = (1 - v.y) * 0.12f;
        float noise = Noise.Sample(v / 10f);
        return heightBias + noise;
    };

    void OnDrawGizmos()
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
                    Vector3[] tris = MarchingCubes.TriangulateCube(new Vector3(x, y, z), field);
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

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMesh();

    }

}
