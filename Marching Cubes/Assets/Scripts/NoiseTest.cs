using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTest : MonoBehaviour
{
    [System.Serializable]
    public struct Octave
    {
        [Range(1, 100)] public float wavelength;
        [Range(1, 100)] public float amplitude;
    }

    // Exposed in unity
    public Material material;

    private List<Octave> octaves = new List<Octave>()
    {
        new Octave() {wavelength = 41.9f, amplitude = 23.5f},
        new Octave() {wavelength = 11.9f, amplitude = 15.0f},
        new Octave() {wavelength = 5.40f, amplitude = 4.50f}
    };

    // Members
    private Vector3Int _chunkSize = new Vector3Int(40, 30, 40);
    private Dictionary<Vector3Int, Chunk> _chunks = new Dictionary<Vector3Int, Chunk>();


    private float Field(Vector3 v)
    {
        float value = 0f;
        float heightBias = 15 - v.y;
        foreach (Octave octave in octaves)
        {
            value += octave.amplitude * Perlin.Noise(v / octave.wavelength);
        }

        return heightBias + value;
    }

    private void Start() => GenerateChunks();

    private async void GenerateChunks()
    {
        for (int x = 0; x < 1; x++)
        {
            for (int z = 0; z < 1; z++)
            {
                Vector3Int chunkId = new Vector3Int(x, 0, z);
                Chunk meshGen = await GenerateChunk(chunkId);
                _chunks.Add(chunkId, meshGen);
            }
        }
    }

    private async Task<Chunk> GenerateChunk(Vector3Int chunkId)
    {
        var chunk = new GameObject($"Chunk [{chunkId.x},{chunkId.z}]");
        var meshGen = chunk.AddComponent<Chunk>();
        chunk.GetComponent<MeshRenderer>().material = material;
        chunk.transform.parent = transform;
        chunk.transform.position = new Vector3(chunkId.x * _chunkSize.x, 0, chunkId.z * _chunkSize.z);
        await meshGen.InitializeAsync(chunkId, _chunkSize, Field);
        return meshGen;
    }
}
