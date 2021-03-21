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
    private Vector3Int chunkSize = new Vector3Int(20, 25, 20);
    
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
        int chunkX = (int)Mathf.Floor(player.transform.position.x / chunkSize.x);
        int chunkZ = (int)Mathf.Floor(player.transform.position.z / chunkSize.z);
        return new Vector3Int(chunkX, 0, chunkZ);
    }

    private IEnumerable<Vector3Int> chunksInRenderDistance()
    {
        var chunks = new List<Vector3Int>();

        for (var x = currentChunk.x - 5; x <= currentChunk.x + 5; x += 1)
        {
            for (var z = currentChunk.z - 5; z <= currentChunk.z + 5; z += 1)
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
        chunk.transform.position = new Vector3(chunkId.x * chunkSize.x, 0, chunkId.z * chunkSize.z);
        MeshGenerator meshGen = chunk.AddComponent<MeshGenerator>();
        chunk.GetComponent<MeshRenderer>().sharedMaterial = material;
        meshGen.Initialize(chunkId, chunkSize);
    }

    private void DestroyChunk(Vector3Int chunkIndex)
    {
        Destroy(chunks[chunkIndex]);
        chunks.Remove(chunkIndex);
    }

}
