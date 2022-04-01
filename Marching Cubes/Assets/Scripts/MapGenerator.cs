using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Unity connected
    public Material material;
    private GameObject player;
    private Vector3Int currentChunk;

    private Dictionary<Vector3Int, GameObject> chunks = new Dictionary<Vector3Int, GameObject>();
    private static Vector3Int _chunkSize = new Vector3Int(20, 25, 20);
    
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
            Debug.Log($"Creating chunk [{newChunk.x}, {newChunk.z}]");
            UpdateChunks();
        }
    }

    private void UpdateChunks()
    {
        var previousChunks = chunks.Keys.ToArray();
        var newChunks = chunksInRenderDistance();

        foreach (Vector3Int chunkIndex in newChunks)
        {
            // Chunk already loaded
            if (chunks.ContainsKey(chunkIndex)) continue;

            // New Chunk
            CreateChunk(chunkIndex);
        }

        foreach (Vector3Int chunkIndex in previousChunks)
        {
            // Remove old chunks
            var newChunkSet = new HashSet<Vector3Int>();
            foreach (Vector3Int i in newChunks)
            {
                newChunkSet.Add(i);
            }
            if (!newChunkSet.Contains(chunkIndex)) DestroyChunk(chunkIndex);
        }
    }

    private MarchingCubes.ScalarField _field = (v) =>
    {
        // Hard floor at 0
        if (v.y <= Mathf.Epsilon) return 1;
        if (v.y >= _chunkSize.y - Mathf.Epsilon) return -1;

        // Soft floor
        float heightBias = (float) Mathf.Pow(((_chunkSize.y / 2f) - v.y) / 4, 3) * 0.1f;

        // Noise octaves
        Vector3 mountains = v / 32;
        float mountainNoise = Perlin.Noise(mountains.x, mountains.y, mountains.z) * 5;
        Vector3 bumps = v / 12;
        float bumpsNoise = Perlin.Noise(bumps.x, bumps.y, bumps.z) * 2;
        Vector3 roughness = v / 3;
        float roughNoise = Perlin.Noise(roughness.x, roughness.y, roughness.z);

        return heightBias + mountainNoise + bumpsNoise + roughNoise;
    };

    private Vector3Int positionToChunkIndex(Vector3 position)
    {
        int chunkX = (int)Mathf.Floor(player.transform.position.x / _chunkSize.x);
        int chunkZ = (int)Mathf.Floor(player.transform.position.z / _chunkSize.z);
        return new Vector3Int(chunkX, 0, chunkZ);
    }

    private IEnumerable<Vector3Int> chunksInRenderDistance()
    {
        var chunks = new List<Vector3Int>();

        for (var x = currentChunk.x - 4; x <= currentChunk.x + 4; x += 1)
        {
            for (var z = currentChunk.z - 4; z <= currentChunk.z + 4; z += 1)
            {
                chunks.Add(new Vector3Int(x, 0, z));
            }
        }
        return chunks;
    }

    private async void CreateChunk(Vector3Int chunkId)
    {
        // perf matters
        Stopwatch stopwatch = Stopwatch.StartNew();

        GameObject chunk = new GameObject($"Chunk {chunkId.x},{chunkId.z}");
        chunks.Add(chunkId, chunk);
        chunk.transform.parent = transform;
        chunk.transform.position = new Vector3(chunkId.x * _chunkSize.x, 0, chunkId.z * _chunkSize.z);
        MeshGenerator meshGen = chunk.AddComponent<MeshGenerator>();
        chunk.GetComponent<MeshRenderer>().sharedMaterial = material;
        await meshGen.InitializeAsync(chunkId, _chunkSize, _field);

        stopwatch.Stop();
        Debug.Log($"Created chunk [{chunkId.x}, {chunkId.z}] ({stopwatch.ElapsedMilliseconds}ms)");
    }

    private void DestroyChunk(Vector3Int chunkIndex)
    {
        Destroy(chunks[chunkIndex]);
        chunks.Remove(chunkIndex);
    }

}
