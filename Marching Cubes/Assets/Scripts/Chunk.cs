using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk
{
    public struct Data
    {
        public Vector3Int chunkId;
        public Vector3[] vertices;
        public Color[] colors;
        public int[] triangles;

        public Data(Vector3Int id) {
            chunkId = id;
            vertices = null;
            colors = null;
            triangles = null;
        }
    }
    
    public static Data GenerateChunk(Vector3Int chunkId, Vector3Int chunkSize, MarchingCubes.ScalarField s, float gridSize = 1)
    {
        if (gridSize != 1) throw new NotImplementedException("Non-unit gridSizing not yet implemented.");

        var chunkData = new Data(chunkId);

        var origin = new Vector3(chunkId.x * chunkSize.x, chunkId.y * chunkSize.y, chunkId.z * chunkSize.z);
        var vertices = new List<Vector3>();

        for (float x = 0; x < chunkSize.x; x += gridSize)
        {
            for (float y = 0; y < chunkSize.y; y += gridSize)
            {
                for (float z = 0; z < chunkSize.z; z += gridSize)
                {
                    Vector3[] tris = MarchingCubes.TriangulateCube(new Vector3(x, y, z), (v) => s(origin + v));
                    vertices.AddRange(tris);
                }
            }
        }

        // Separate this later as another delegate
        Color[] colors = new Color[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            colors[i] = Color.Lerp(Color.blue, Color.red, (vertices[i].y / 10f));
        }

        chunkData.vertices = vertices.ToArray();
        chunkData.colors = colors;
        chunkData.triangles = Enumerable.Range(0, vertices.Count).ToArray();

        return chunkData;
    }

}
