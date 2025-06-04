using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using Debug = VictorDev.Common.Debug;
using Random = UnityEngine.Random;

namespace VictorDev.ShaderUtils
{
    [RequireComponent(typeof(BoxCollider), typeof(HLSLRendering))]
    public class HeatMapGeneratorHLSL : MonoBehaviour
    {
        [Button]
        public void Clear() => HlslRendering.ClearMesh();

        [Button]
        public void Generate()
        {
            Clear();

            Vector3 boundSize = BoxColliderTarget.size;
            Vector3 center = BoxColliderTarget.center;
            Vector3 worldStart = BoxColliderTarget.transform.TransformPoint(center - boundSize * 0.5f);
            Vector3 worldSize = Vector3.Scale(boundSize, BoxColliderTarget.transform.lossyScale);
            
            //先計算數量，以int跑迴圈會比float跑迴圈更省效能
            int countX = Mathf.FloorToInt(worldSize.x / spacing);
            int countY = Mathf.FloorToInt(worldSize.y / spacing);
            int countZ = Mathf.FloorToInt(worldSize.z / spacing);
            
            Debug.Log($"網格數量：{countX*countY*countZ} / {countX}:{countY}:{countZ}", this, EmojiEnum.DataBox);

            int batchCounter = 0;
            int batchSize = 1000; //批次處理
            
            if(_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(DrawMeshCoroutine());
            
            IEnumerator DrawMeshCoroutine()
            {
                for (int xi = 0; xi <= countX; xi++)
                {
                    float x = xi * spacing;
                    for (int yi = 0; yi <= countY; yi++)
                    {
                        float y = yi * spacing;
                        for (int zi = 0; zi <= countZ; zi++)
                        {
                            float z = zi * spacing;
                            Vector3 localPos = new Vector3(x, y, z);
                            Vector3 worldPos = worldStart + localPos;

                            if (BoxColliderTarget.bounds.Contains(worldPos))
                            {
                                Color baseColor = Color.Lerp(minColor, maxColor, 0);
                                HlslRendering.DrawMesh(worldPos, baseColor, emissonIntensity, meshSize);
                            }

                            // 可選：每處理 batchSize 個格子就讓出一幀
                            if (++batchCounter >= batchSize)
                            {
                                batchCounter = 0;
                                yield return null;
                            }
                        }
                    }
                }
            }
        }

        private void OnValidate()
        {
            minValue = Mathf.Min(minValue, maxValue);
            maxValue = Mathf.Max(minValue, maxValue);
        }

        #region Variables
        [Header(">>> 尺吋")]
        [SerializeField] float meshSize = 1f;
        [Header(">>> 間距")]
        [SerializeField] float spacing = 0.5f; // 間隔大小（格子大小）
        [Header(">>> 發光程度(會影響透明度)")]
        [SerializeField] float emissonIntensity = 0f;
        
        [Foldout("[設定] - 最小值、最大值與顏色")]
        [SerializeField] float minValue = 0, maxValue = 100;
        [Foldout("[設定] - 最小值、最大值與顏色")]
        [SerializeField] Color minColor = new Color(0, 1, 0, 1/255f), maxColor = new Color(1, 0, 0, 1/255f);
        
        private Coroutine _coroutine;
        private HLSLRendering HlslRendering => _hlslRendering ??= GetComponent<HLSLRendering>();
        [NonSerialized] private HLSLRendering _hlslRendering;
        private BoxCollider BoxColliderTarget => _boxColliderTarget ??= GetComponent<BoxCollider>();
        [NonSerialized] private BoxCollider _boxColliderTarget;
        #endregion
    }
}