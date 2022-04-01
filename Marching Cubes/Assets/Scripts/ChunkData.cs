using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkData
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

    private static float[,,] SampleScalarField(Vector3Int chunkId, Vector3Int chunkSize, MarchingCubes.ScalarField s)
    {
        Vector3 origin = new Vector3(chunkId.x * chunkSize.x, chunkId.y * chunkSize.y, chunkId.z * chunkSize.z);
        float[,,] data = new float[(chunkSize.x + 1), chunkSize.y + 1, chunkSize.z + 1];
        for (int x = 0; x <= chunkSize.x; x++)
        {
            for (int y = 0; y <= chunkSize.y; y++)
            {
                for (int z = 0; z <= chunkSize.z; z++)
                {
                    data[x,y,z] = s(origin + new Vector3(x, y, z));
                }
            }
        }

        return data;
    }

    public static Data GenerateChunkData(Vector3Int chunkId, Vector3Int chunkSize, MarchingCubes.ScalarField s, int gridSize = 1)
    {
        if (gridSize != 1) throw new NotImplementedException("Non-unit gridSizing not yet implemented.");

        var chunkData = new Data(chunkId);
        var chunkValues = SampleScalarField(chunkId, chunkSize, s);

        var vertices = new List<Vector3>();

        for (int x = 0; x < chunkSize.x; x += gridSize)
        {
            for (int y = 0; y < chunkSize.y; y += gridSize)
            {
                for (int z = 0; z < chunkSize.z; z += gridSize)
                {
                    Vector3[] tris =
                        MarchingCubes.TriangulateCube(new Vector3(x, y, z), (v) => chunkValues[(int)v.x, (int)v.y, (int)v.z]);
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
