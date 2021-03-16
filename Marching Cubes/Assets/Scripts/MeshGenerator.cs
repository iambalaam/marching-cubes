using System.Collections;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    MarchingCubes.ScalarField field = (Vector3 v) => {
        float heightBias = (1 - v.y) * 0.12f;
        float noise = Noise.Sample(v / 10f);
        return heightBias + noise;
    };

    public void Initialize(Vector3Int chunkId)
    {
        StartCoroutine(GenerateChunkOnNewThread(chunkId));
    }

    IEnumerator GenerateChunkOnNewThread(Vector3Int chunkId)
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Chunk.Data data = new Chunk.Data(chunkId);

        Thread chunkGeneration = new Thread(() => {
            data = Chunk.GenerateChunk(chunkId, field);
        });
        chunkGeneration.Start();

        while (chunkGeneration.IsAlive) yield return null;

        mesh.vertices = data.vertices;
        mesh.colors = data.colors;
        mesh.triangles = data.triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    
}
