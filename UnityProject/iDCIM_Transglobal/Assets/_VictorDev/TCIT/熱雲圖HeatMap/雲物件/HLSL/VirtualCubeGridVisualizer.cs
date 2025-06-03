using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VirtualCubeGridVisualizer : MonoBehaviour
{
    public Material visualizerMaterial;

    [Header("Grid Settings")]
    public Vector3 gridSize = new Vector3Int(8, 8, 8);
    public Vector3 cubeSize = new Vector3(0.5f, 1f, 0.5f);
    public float spacing = 0.1f;

    [Header("Color Gradient")]
    public Color colorLow = Color.blue;
    public Color colorHigh = Color.red;

    [Header("Visual Settings")]
    public float heightScale = 1f;
    public float emissionStrength = 1f;

    private MeshRenderer meshRenderer;

    private void OnEnable()
    {
        SetupMesh();
        ApplyMaterialProperties();
    }

    private void Update()
    {
        ApplyMaterialProperties();
    }

    private void SetupMesh()
    {
        // Create a single large quad facing up to render the effect
        var meshFilter = GetComponent<MeshFilter>();
        var mesh = new Mesh();

        mesh.vertices = new Vector3[]
        {
            new Vector3(-1, 0, -1),
            new Vector3(1, 0, -1),
            new Vector3(1, 0, 1),
            new Vector3(-1, 0, 1),
        };

        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
        };

        mesh.triangles = new int[]
        {
            0, 1, 2,
            2, 3, 0
        };

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        // Scale the quad to cover grid area
        transform.localScale = new Vector3(
            gridSize.x * (cubeSize.x + spacing),
            1f,
            gridSize.z * (cubeSize.z + spacing)
        );
    }

    private void ApplyMaterialProperties()
    {
        if (visualizerMaterial == null) return;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = visualizerMaterial;

        visualizerMaterial.SetVector("_GridSize", gridSize);
        visualizerMaterial.SetVector("_CubeSize", cubeSize);
        visualizerMaterial.SetFloat("_Spacing", spacing);
        visualizerMaterial.SetColor("_ColorLow", colorLow);
        visualizerMaterial.SetColor("_ColorHigh", colorHigh);
        visualizerMaterial.SetFloat("_HeightScale", heightScale);
        visualizerMaterial.SetFloat("_EmissionStrength", emissionStrength);
    }
}
