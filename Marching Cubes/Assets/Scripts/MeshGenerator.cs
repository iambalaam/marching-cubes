using System;
using System.Collections;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    private Vector3Int _chunkSize = Vector3Int.zero;

    private void OnDrawGizmosSelected()
    {
        if (_chunkSize != Vector3Int.zero)
        {
            Gizmos.DrawWireCube(transform.position + _chunkSize / 2, _chunkSize);
        }
    }

    public void Initialize(Vector3Int chunkId, Vector3Int chunkSize, MarchingCubes.ScalarField field)
    {
        StartCoroutine(GenerateChunkOnNewThread(chunkId, chunkSize, field));
    }

    IEnumerator GenerateChunkOnNewThread(Vector3Int chunkId, Vector3Int chunkSize, MarchingCubes.ScalarField field)
    {
        var meshFilter = GetComponent<MeshFilter>();
        var meshCollider = GetComponent<MeshCollider>();

        _chunkSize = chunkSize;

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) mesh = new Mesh();
        mesh.name = $"Chunk Mesh: [{chunkId.x},{chunkId.z}]";

        Chunk.Data data = new Chunk.Data(chunkId);

        Thread chunkGeneration = new Thread(() => {
            data = Chunk.GenerateChunk(chunkId, chunkSize, field);
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
