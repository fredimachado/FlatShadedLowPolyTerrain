using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private int size;

    private Vector3[] vertices;
    private int[] triangles;

    private void Start()
    {
        vertices = new Vector3[(size + 1) * (size + 1)];
        for (int i = 0, z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++, i++)
            {
                vertices[i] = new Vector3(x, 0f, z);
            }
        }

        triangles = new int[size * size * 6];
        for (int ti = 0, vi = 0, y = 0; y < size; y++, vi++)
        {
            for (int x = 0; x < size; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + size + 1;
                triangles[ti + 5] = vi + size + 2;
            }
        }

        FlatShading();

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.colors = GetRandomColors(vertices.Length);

        mesh.RecalculateNormals();


        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
    }

    private Color[] GetRandomColors(int vertexCount)
    {
        var colorPalette = new[]
        {
            new Color(1f, 0, 0),
            new Color(0, 1f, 0),
            new Color(0, 0, 1f),
            new Color(1f, 1f, 0),
            new Color(0, 1f, 1f),
            new Color(1f, 0, 1f)
        };

        var colors = new Color[vertexCount];
        var randomIndex = 0;

        for (int i = 0; i < vertexCount; i++)
        {
            // Each triangle has 3 vertices, so, if i % 3 equals 0, then we are in a new triangle
            // so, get a new random color
            if (i % 3 == 0)
            {
                randomIndex = Random.Range(0, colorPalette.Length - 1);
            }

            colors[i] = colorPalette[randomIndex];
        }

        return colors;
    }

    private void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
    }
}
