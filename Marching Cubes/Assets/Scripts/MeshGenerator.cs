using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
// [RequireComponent(typeof(MeshCollider))]
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
        GenerateChunk(chunkId, chunkSize, field);
    }

    private void GenerateChunk(Vector3Int chunkId, Vector3Int chunkSize, MarchingCubes.ScalarField field)
    {
        var meshFilter = GetComponent<MeshFilter>();
        // var meshCollider = GetComponent<MeshCollider>();

        _chunkSize = chunkSize;

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) mesh = new Mesh();
        mesh.name = $"Chunk Mesh: [{chunkId.x},{chunkId.z}]";

        Chunk.Data data = new Chunk.Data(chunkId);

        data = Chunk.GenerateChunkOld(chunkId, chunkSize, field);

        mesh.Clear();
        mesh.vertices = data.vertices;
        mesh.colors = data.colors;
        mesh.triangles = data.triangles;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        //meshCollider.sharedMesh = mesh;
    }

    public async Task<bool> InitializeAsync(Vector3Int chunkId, Vector3Int chunkSize, MarchingCubes.ScalarField field)
    {
        return await GenerateAsyncChunk(chunkId, chunkSize, field);
    }


    async Task<bool> GenerateAsyncChunk(Vector3Int chunkId, Vector3Int chunkSize, MarchingCubes.ScalarField field)
    {
        var meshFilter = GetComponent<MeshFilter>();
        //var meshCollider = GetComponent<MeshCollider>();

        _chunkSize = chunkSize;

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) mesh = new Mesh();
        mesh.name = $"Chunk Mesh: [{chunkId.x},{chunkId.z}]";

        Chunk.Data data = new Chunk.Data(chunkId);
        data = await Task.Run(() => { return Chunk.GenerateChunk(chunkId, chunkSize, field); });
        
        mesh.Clear();
        mesh.vertices = data.vertices;
        mesh.colors = data.colors;
        mesh.triangles = data.triangles;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        //meshCollider.sharedMesh = mesh;

        return true;
    }

}