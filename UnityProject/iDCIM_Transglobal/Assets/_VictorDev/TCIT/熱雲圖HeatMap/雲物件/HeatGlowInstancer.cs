using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class HeatGlowInstancer : MonoBehaviour
{
    public Mesh mesh; // 小方塊 mesh
    public Material material; // 用的材質
    private BoxCollider BoxColliderTarget => _boxColliderTarget ??= GetComponent<BoxCollider>();
    [NonSerialized] private BoxCollider _boxColliderTarget;
    public float spacing = 0.5f; // 間隔大小（格子大小）

    private MaterialPropertyBlock propertyBlock;

    private List<Matrix4x4> matrices;
    private List<Vector4> colors;
    private List<float> glows;

    public Color minColor = new Color(0, 1, 0, 0.3f), maxColor = new Color(1, 0, 0, 0.3f);
    public float intensity = 1f;

    [Button]
    void Start()
    {
        matrices = new List<Matrix4x4>();
        colors = new List<Vector4>();
        glows = new List<float>();

        propertyBlock = new MaterialPropertyBlock();

        Vector3 size = BoxColliderTarget.size;
        Vector3 center = BoxColliderTarget.center;
        Vector3 worldStart = BoxColliderTarget.transform.TransformPoint(center - size * 0.5f);
        Vector3 worldSize = Vector3.Scale(size, BoxColliderTarget.transform.lossyScale);

        // 取得區域的大小與起點位置（世界座標）
        for (float x = 0; x <= worldSize.x; x += spacing)
        {
            for (float y = 0; y <= worldSize.y; y += spacing)
            {
                for (float z = 0; z <= worldSize.z; z += spacing)
                {
                    Vector3 localPos = new Vector3(x, y, z);
                    Vector3 worldPos = worldStart + localPos;
                    // 檢查是否在 BoxColliderTargetlider 的範圍內（可略過）
                    if (BoxColliderTarget.bounds.Contains(worldPos))
                    {
                        matrices.Add(Matrix4x4.TRS(worldPos, Quaternion.identity, Vector3.one * 0.2f));

                        // 模擬熱度，隨機值決定顏色與發光強度
                        float heat = Random.value;
                        heat = 0;
                        Color baseColor = Color.Lerp(minColor, maxColor, heat);
                        float glow = Mathf.Lerp(0f, intensity, heat); // alpha 代表發光強度

                        colors.Add(new Vector4(baseColor.r, baseColor.g, baseColor.b, baseColor.a));
                        glows.Add(glow); // glow 控制發光強度
                    }
                }
            }
            propertyBlock.SetVectorArray("_Color", colors);
            propertyBlock.SetFloatArray("_GlowIntensity", glows);
        }
    }

        void Update()
        {
            // 繪製所有實例
            Graphics.DrawMeshInstanced(mesh, 0, material, matrices.ToArray(), matrices.Count, propertyBlock,
                ShadowCastingMode.Off, false, 0, null,
                LightProbeUsage.Off, null);
        }
}