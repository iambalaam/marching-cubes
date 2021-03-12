using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    // Unity connected
    [Range(-2, 2)]
    public float sOffset = 0;
    
    private float zOffset = 0;

    Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        zOffset += Time.deltaTime;

        Vector3Int mapSize = new Vector3Int(30, 15, 30);

        MarchingCubes.ScalarField field = (Vector3 v) => {
            // Hard floor
            if (v.y == 15) return -0.01f;
            // Adding nice edges to the map
            if (v.x == 0 || v.x == mapSize.x -1) return 0.01f;
            if (v.z == 0 || v.z == mapSize.z -1) return 0.01f;

            v.z += zOffset;
            float heightBias = (8 - v.y) * 0.12f;
            float noise = Noise.Sample(v / 10f);
            return heightBias + noise + sOffset;
        };


        List<Vector3> vertices = new List<Vector3>();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    vertices.AddRange(MarchingCubes.TriangulateCube(new Vector3(x, y, z), field));

                }
            }
        }

        int[] triangles = Enumerable.Range(0, vertices.Count).ToArray();
        Color[] colors = new Color[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            colors[i] = Color.Lerp(Color.blue, Color.red, (vertices[i].y / 5f) + 3f);
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }

}
