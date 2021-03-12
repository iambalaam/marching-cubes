using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    // Unity connected
    [Range(0, 100)]
    public float zOffset = 0;

    Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        zOffset += Time.deltaTime;

        Vector3Int mapSize = new Vector3Int(15, 15, 15);

        MarchingCubes.ScalarField field = (Vector3 v) => {
            v.z += zOffset;
            float floor = (8 - v.y) * 0.1f;
            float noise = Noise.Sample(v / 10f);
            return floor + noise;
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

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }

}
