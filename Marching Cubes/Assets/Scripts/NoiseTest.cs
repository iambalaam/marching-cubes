using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTest : MonoBehaviour
{
    // Exposed in unity
    public Material material;

    // Members
    private Vector3Int _chunkSize = new Vector3Int(20, 50, 20);

    private void Start() => GenerateChunks();

    private void GenerateChunks()
    {
        var chunk = new GameObject($"Chunk");
        var meshGen = chunk.AddComponent<MeshGenerator>();
        chunk.GetComponent<MeshRenderer>().material = material;
        chunk.transform.parent = transform;
        meshGen.Initialize(Vector3Int.zero, _chunkSize);
    }
}
