using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteAlways]
public class Chunk : MonoBehaviour
{
    private static Vector3Int chunkSize = new Vector3Int(50, 10, 50);
    private static float gridSize = 1f;

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
        Gizmos.DrawWireCube(transform.position + ((Vector3)chunkSize / 2f), chunkSize);
    }

    private void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();

        for (float x = 0; x < chunkSize.x; x += gridSize)
        {
            for (float y = 0; y < chunkSize.y; y += gridSize)
            {
                for (float z = 0; z < chunkSize.z; z += gridSize)
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
