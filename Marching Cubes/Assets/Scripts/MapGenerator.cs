﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Unity connected
    public Material material;
    private GameObject player;
    private Vector3Int currentChunk;

    private Dictionary<Vector3Int, GameObject> chunks = new Dictionary<Vector3Int, GameObject>();

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentChunk = positionToChunkIndex(player.transform.position);

        foreach (Vector3Int chunkIndex in chunksInRenderDistance())
        {
            CreateChunk(chunkIndex);
        }
    }

    private void Update()
    {
        Vector3Int newChunk = positionToChunkIndex(player.transform.position);
        // Player has moved chunk
        if (newChunk != currentChunk)
        {
            currentChunk = newChunk;
            var previousChunks = chunks.Keys.ToArray();
            var newChunks = chunksInRenderDistance();
            foreach (Vector3Int chunkIndex in newChunks)
            {
                // Chunk already loaded
                if (chunks.ContainsKey(chunkIndex)) continue;

                // New Chunk
                CreateChunk(chunkIndex);
            }

            foreach(Vector3Int chunkIndex in previousChunks)
            {
                // Remove old chunks
                if (!newChunks.Contains(chunkIndex)) DestroyChunk(chunkIndex);
            }

        }
    }

    private Vector3Int positionToChunkIndex(Vector3 position)
    {
        int chunkX = (int)Mathf.Floor(player.transform.position.x / Chunk.chunkSize.x);
        int chunkZ = (int)Mathf.Floor(player.transform.position.z / Chunk.chunkSize.z);
        return new Vector3Int(chunkX, 0, chunkZ);
    }

    private IEnumerable<Vector3Int> chunksInRenderDistance()
    {
        List<Vector3Int> chunks = new List<Vector3Int>();

        for (int x = currentChunk.x - 1; x <= currentChunk.x + 1; x += 1)
        {
            for (int z = currentChunk.z - 1; z <= currentChunk.z + 1; z += 1)
            {
                chunks.Add(new Vector3Int(x, 0, z));
            }
        }
        return chunks;
    }

    private void CreateChunk(Vector3Int chunkIndex)
    {
        GameObject chunk = new GameObject($"Chunk {chunkIndex.x},{chunkIndex.z}");
        chunks.Add(chunkIndex, chunk);
        chunk.transform.parent = transform;
        chunk.transform.position = new Vector3(chunkIndex.x * Chunk.chunkSize.x, 0, chunkIndex.z * Chunk.chunkSize.z);
        chunk.AddComponent<Chunk>();
        chunk.GetComponent<MeshRenderer>().material = material;
    }

    private void DestroyChunk(Vector3Int chunkIndex)
    {
        Destroy(chunks[chunkIndex]);
        chunks.Remove(chunkIndex);
    }

}
