using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace VictorDev.ShaderUtils
{
    /// 負責繪制HLSL Graphics.DrawMeshInstance
    /// <para> + 它是繪製圖形，並非真實Instance，所以可以根據Matrix4x4[]設定的內容進行繪製多個MeshInstance</para>
    public class HLSLRenderer : MonoBehaviour
    {
        public bool IsActived
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }

        private void Awake()
        {
            _propertyBlock ??= new MaterialPropertyBlock();
            _matrices ??= new List<Matrix4x4>();
            _colors ??= new List<Vector4>();
            _glowIntensity ??= new List<float>();
        }

        /// 繪製MeshInstance網格
        public Matrix4x4 DrawMeshInstance(Vector3 pos, Color color, float emissionIntensity = 0, float? size = null)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * (size ?? meshSize));
            SetMeshInstanceColor(matrix, color, emissionIntensity);
            return matrix;
        }

        /// 設置點位顏色
        public void ChangeMeshInstanceColor(Matrix4x4 matrix, Color color, float emissionIntensity = 0)
        {
            RemoveMeshInstance(matrix);
            SetMeshInstanceColor(matrix, color, emissionIntensity);
        }

        private void SetMeshInstanceColor(Matrix4x4 matrix, Color color, float emissionIntensity = 0)
        {
            _propertyBlock.Clear();
            _matrices.Add(matrix);
            _colors.Add(new Vector4(color.r, color.g, color.b, color.a)); //顏色
            _glowIntensity.Add(emissionIntensity); // glow 控制發光強度

            _propertyBlock.SetVectorArray(PropertyBlockColor, _colors);
            _propertyBlock.SetFloatArray(PropertyBlockGlowIntensity, _glowIntensity);
        }

        /// 移除點位
        private void RemoveMeshInstance(Matrix4x4 matrix)
        {
            if (_matrices.Contains(matrix))
            {
                int pointIndex = _matrices.IndexOf(matrix);
                _matrices.RemoveAt(pointIndex);
                _colors.RemoveAt(pointIndex);
                _glowIntensity.RemoveAt(pointIndex);
            }
        }

        /// 清除全部MeshInstance
        [Button]
        public void ClearMeshInstance()
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

        private string PropertyBlockColor => "_Color";
        private string PropertyBlockGlowIntensity => "_GlowIntensity";

        /// 每個點位資訊的的Matrix4x4
        public List<Matrix4x4> Matrices => _matrices;

        #endregion

        #region For測試

        [Foldout("[測試]")] public Vector3 testPos;
        [Foldout("[測試]")] public Color testColor = new Color(0f, 1f, 0f, 0.5f);
        [Foldout("[測試]")] public float testEmission = 1f;

        [Button]
        private void DrawMesh() => DrawMeshInstance(testPos, testColor, testEmission, meshSize);

        [Button]
        private void RemoveLast() => RemoveMeshInstance(Matrices[^1]);

        #endregion
    }
}