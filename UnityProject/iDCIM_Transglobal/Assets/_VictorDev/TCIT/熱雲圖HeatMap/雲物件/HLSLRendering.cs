using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace VictorDev.ShaderUtils
{
    /// 負責繪制HLSL
    /// <para> + 它是繪製圖形，並非instance，所以可以根據陣列設定的內容進行多個繪製</para>
    public class HLSLRendering : MonoBehaviour
    {
        public bool IsActived
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }

        public void DrawMesh(Vector3 pos, Color color, float emissionIntensity = 0, float? size = null)
        {
            _propertyBlock ??= new MaterialPropertyBlock();
            _propertyBlock.Clear();
            _matrices ??= new List<Matrix4x4>();
            _matrices.Add(Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * (size ?? meshSize)));
            
            _colors ??= new List<Vector4>();
            _colors.Add(new Vector4(color.r, color.g, color.b, color.a)); //顏色
            _glowIntensity ??= new List<float>();
            _glowIntensity.Add(emissionIntensity); // glow 控制發光強度

            _propertyBlock.SetVectorArray("_Color", _colors);
            _propertyBlock.SetFloatArray("_GlowIntensity", _glowIntensity);
        }

        [Button]
        private void Remove()
        {
            _matrices.RemoveAt(1);
            _colors.RemoveAt(1);
            _glowIntensity.RemoveAt(1);
        }

        [Button]
        private void DrawMesh() => DrawMesh(testPos, testColor, testEmission, meshSize);
        [Button]
        public void ClearMesh()
        {
            _propertyBlock?.Clear();
            _matrices?.Clear();
            _colors?.Clear();
            _glowIntensity?.Clear();
        }

        void Update()
        {
            if (_matrices is { Count: > 0 })
            {
                // 繪製所有實例
                Graphics.DrawMeshInstanced(mesh, 0, material, _matrices.ToArray(), _matrices.Count, _propertyBlock,
                    ShadowCastingMode.Off, false, 0, null,
                    LightProbeUsage.Off, null);
            }
        }

        #region Variables
        [Header(">>> 繪製尺吋")] [MinValue(0.1f)] [SerializeField]
        float meshSize = 1f;
        [Header("[繪製的形狀與HLSL的Shader材質]")] [SerializeField]
        Mesh mesh;
        [SerializeField] Material material;

        private MaterialPropertyBlock _propertyBlock;
        private List<Matrix4x4> _matrices;
        private List<Vector4> _colors;
        private List<float> _glowIntensity;
        
        /// 每個Mesh點的座標
        public List<Vector3> MeshPos => _matrices.Select(m=> m.GetPosition()).ToList();
        #endregion

        #region For測試
        [Foldout("[測試]")] public Vector3 testPos;
        [Foldout("[測試]")] public Color testColor = new Color(0f, 1f, 0f, 0.5f);
        [Foldout("[測試]")] public float testEmission = 1f;
        #endregion
    }
}