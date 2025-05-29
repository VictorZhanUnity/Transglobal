using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class HeatGlowInstancer : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public int count = 100;
    public Vector3 areaSize = new Vector3(10, 10, 10);
    public float spacing = 0.5f;       // 間隔大小（格子大小）

    private Matrix4x4[] matrices;
    private Vector4[] colors;
    private float[] glows;
    private MaterialPropertyBlock propertyBlock;

    public Color minColor = new Color(0, 1, 0, 0.3f), maxColor= new Color (1, 0, 0, 0.3f);
    public float intensity = 1f;
    
    [Button]
    void Start()
    {
        matrices = new Matrix4x4[count];
        colors = new Vector4[count];
        glows = new float[count];
        propertyBlock = new MaterialPropertyBlock();
        
        BoxCollider boxCol = GetComponent<BoxCollider>();

        // BoxCollider 的尺寸（local space）
        Vector3 boxSize = boxCol.size;

        // BoxCollider 的中心（local space）
        Vector3 boxCenter = boxCol.center;

        // BoxCollider 在世界座標
        Vector3 worldPos = boxCol.transform.position;
        Quaternion worldRot = boxCol.transform.rotation;
        Vector3 worldScale = boxCol.transform.lossyScale;
        

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(-areaSize.y / 2, areaSize.y / 2),
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );
            matrices[i] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * 0.2f);

            // 模擬熱度，隨機值決定顏色與發光強度
            float heat = Random.value;
            Debug.Log($"heat: {heat}");
            Color baseColor = Color.Lerp(minColor, maxColor, heat);
            float glow = Mathf.Lerp(0f, intensity, heat); // alpha 代表發光強度
            
            colors[i] = new Vector4(baseColor.r, baseColor.g, baseColor.b, baseColor.a);
            glows[i] = glow; // glow 控制發光強度
        }

        propertyBlock.SetVectorArray("_Color", colors);
        propertyBlock.SetFloatArray("_GlowIntensity", glows);
    }

    void Update()
    {
        // 繪製所有實例
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, count, propertyBlock,
            UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null,
            UnityEngine.Rendering.LightProbeUsage.Off, null);
    }
}