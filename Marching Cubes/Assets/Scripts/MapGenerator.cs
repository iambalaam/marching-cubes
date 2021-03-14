using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Unity connected
    public Material material;

    void Start()
    {
        for (int x = 0; x < 100; x+=50)
        {
            for (int z = 0; z < 100; z+=50)
            {
                GameObject chunk = new GameObject($"Chunk {x},{z}");
                chunk.transform.parent = transform;
                chunk.transform.position = new Vector3(x, 0, z);
                chunk.AddComponent<Chunk>();
                chunk.GetComponent<MeshRenderer>().material = material;
            }
        }
    }
}
