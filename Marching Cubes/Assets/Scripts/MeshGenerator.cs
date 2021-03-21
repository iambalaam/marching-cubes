using System;
using System.Collections;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    private Vector3Int chunkSize = Vector3Int.zero;

    MarchingCubes.ScalarField generateField (Vector3Int chunkSize) {
        return (v) =>
        {
            // Hard floor at 0
            if (v.y == 0) return 1;
            if (v.y == chunkSize.y) return -1;

            // Soft floor
            float heightBias = (float) Math.Pow(((chunkSize.y / 2) - v.y) / 4, 3) * 0.1f;

            // Noise octaves
            Vector3 mountains = v / 32;
            float mountainNoise = Perlin.Noise(mountains.x, mountains.y, mountains.z) * 5;
            Vector3 bumps = v / 12;
            float bumpsNoise = Perlin.Noise(bumps.x, bumps.y, bumps.z) * 2;
            Vector3 roughness = v / 3;
            float roughNoise = Perlin.Noise(roughness.x, roughness.y, roughness.z);

            return heightBias + mountainNoise + bumpsNoise + roughNoise;
        };
    }

    private void OnDrawGizmos()
    {
        if (chunkSize != Vector3Int.zero)
        {
            Gizmos.DrawWireCube(transform.position + chunkSize / 2, chunkSize);
        }
    }

    public void Initialize(Vector3Int chunkId, Vector3Int chunkSize)
    {
        StartCoroutine(GenerateChunkOnNewThread(chunkId, chunkSize));
    }

    IEnumerator GenerateChunkOnNewThread(Vector3Int chunkId, Vector3Int chunkSize)
    {
        var meshFilter = GetComponent<MeshFilter>();
        var meshCollider = GetComponent<MeshCollider>();

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) mesh = new Mesh();
        mesh.name = $"Chunk Mesh: [{chunkId.x},{chunkId.z}]";

        Chunk.Data data = new Chunk.Data(chunkId);

        Thread chunkGeneration = new Thread(() => {
            data = Chunk.GenerateChunk(chunkId, chunkSize, generateField(chunkSize));
        });
        chunkGeneration.Start();

        while (chunkGeneration.IsAlive) yield return null;

        mesh.Clear();
        mesh.vertices = data.vertices;
        mesh.colors = data.colors;
        mesh.triangles = data.triangles;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
    
}
