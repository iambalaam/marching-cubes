using System;
using System.Collections;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    MarchingCubes.ScalarField field = (Vector3 v) => {
        // Hard floor at 0
        if (v.y == 0) return 1;
        if (v.y == Chunk.chunkSize.y) return -1;

        // Soft floor
        float heightBias = (float) Math.Pow(((Chunk.chunkSize.y / 2) - v.y) / 4, 3) * 0.1f;

        // Noise octaves
        Vector3 mountains = v / 32;
        float mountainNoise = Perlin.Noise(mountains.x, mountains.y, mountains.z) * 5;
        Vector3 bumps = v / 12;
        float bumpsNoise = Perlin.Noise(bumps.x, bumps.y, bumps.z) * 2;
        Vector3 roughness = v / 3;
        float roughNoise = Perlin.Noise(roughness.x, roughness.y, roughness.z);

        return heightBias + mountainNoise + bumpsNoise + roughNoise;
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
