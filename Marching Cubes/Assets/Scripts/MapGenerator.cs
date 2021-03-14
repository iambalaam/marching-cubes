using UnityEngine;

[ExecuteAlways]
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
                GameObject meshGen = new GameObject();
                meshGen.transform.position = new Vector3(x, 0, z);
                meshGen.AddComponent<MeshGenerator>();
                meshGen.GetComponent<MeshRenderer>().material = material;
            }
        }
    }
}
