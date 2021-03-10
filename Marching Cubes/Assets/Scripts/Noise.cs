using UnityEditor.PackageManager.UI;
using UnityEngine;

public static class Noise
{
    public static float Sample(Vector3 v)
    {
        return Sample(v.x, v.y, v.z);
    }
    public static float Sample(float x, float y, float z)
    {
        // From CarlPilot
        // https://www.youtube.com/watch?v=Aga0TBJkchM
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);

        return (xy + yz + xz) / 3;
    }
}
