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

        for (int x = currentChunk.x - 5; x <= currentChunk.x + 5; x += 1)
        {
            for (int z = currentChunk.z - 5; z <= currentChunk.z + 5; z += 1)
            {
                chunks.Add(new Vector3Int(x, 0, z));
            }
        }
        return chunks;
    }

    private void CreateChunk(Vector3Int chunkId)
    {
        GameObject chunk = new GameObject($"Chunk {chunkId.x},{chunkId.z}");
        chunks.Add(chunkId, chunk);
        chunk.transform.parent = transform;
        chunk.transform.position = new Vector3(chunkId.x * Chunk.chunkSize.x, 0, chunkId.z * Chunk.chunkSize.z);
        MeshGenerator meshGen = chunk.AddComponent<MeshGenerator>();
        chunk.GetComponent<MeshRenderer>().sharedMaterial = material;
        meshGen.Initialize(chunkId);
    }

    private void DestroyChunk(Vector3Int chunkIndex)
    {
        Destroy(chunks[chunkIndex]);
        chunks.Remove(chunkIndex);
    }

}
