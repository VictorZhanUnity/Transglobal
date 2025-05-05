using UnityEngine;
using UnityEngine.Rendering;

namespace VictorDev.MaterialUtils
{
    public static class MaterialHelper
    {
        /// 将材质设置为透明模式
        public static void SetTransparentMode(Material targetMaterial)
        {
            targetMaterial.SetFloat("_Mode", 3); // 设置模式为 Transparent
            targetMaterial.SetOverrideTag("RenderType", "Transparent");
            targetMaterial.EnableKeyword("_ALPHABLEND_ON");
            targetMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            targetMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            targetMaterial.SetInt("_ZWrite", 0); // 关闭深度写入
            targetMaterial.renderQueue = (int)RenderQueue.Transparent; // 设置渲染队列为透明层
        }

        /// 将材质设置为不透明模式
        public static void SetOpaqueMode(Material targetMaterial)
        {
            targetMaterial.SetFloat("_Mode", 0); // 设置模式为 Opaque
            targetMaterial.SetOverrideTag("RenderType", "Opaque");
            targetMaterial.DisableKeyword("_ALPHABLEND_ON");
            targetMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            targetMaterial.SetInt("_DstBlend", (int)BlendMode.Zero);
            targetMaterial.SetInt("_ZWrite", 1); // 开启深度写入
            targetMaterial.renderQueue = (int)RenderQueue.Geometry; // 设置渲染队列为几何层
        }
    }
}