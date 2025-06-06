using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using VictorDev.ShaderUtils;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.HeatMapUtiils
{
    [RequireComponent(typeof(BoxCollider), typeof(HLSLRendererDictionary))]
    public class HeatMapGeneratorHLSL : MonoBehaviour
    {
        [Button]
        private void SetHeapMapPoint() => heatMapSetting.TestHeatMapPoint.ForEach(SetHeapMapPoints);

        /// 設定點位值
        public void SetHeapMapPoints(HeatMapPoint heatMapPoint)
        {
            if (_isGenerateFinished == false) return;
            //找出在範圍內的Item，並從近到遠排列
            List<HeatMapItemHLSL> heatMapItemInRange = _heatMapMatrixList
                .Where(matrix => Vector3.Distance(matrix.Position, heatMapPoint.transform.position) < heatMapSetting.RadiusRange)
                .OrderBy(matrix => Vector3.Distance(matrix.Position, heatMapPoint.transform.position))
                .ToList();

            heatMapPoint.SetHeatMapItemInRange(heatMapItemInRange);
        }

        [Button]
        public void Clear()
        {
            _isGenerateFinished = false;
            if (_coroutine != null) StopCoroutine(_coroutine);
            HlslRenderer.ClearMeshInstance();
            heatMapSetting.TestHeatMapPoint.ForEach(heatMapPoint=> heatMapPoint.ClearHeatMapItemInRange());
        }

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

            Debug.Log($"產生HLSL熱雲圖...", this, EmojiEnum.Gear);
            Debug.Log($"雲格數量：{countX * countY * countZ} / X:{countX} Y:{countY} Z:{countZ}", this, EmojiEnum.DataBox);

            int batchCounter = 0;

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
                                float value = 0;
                                Color baseColor = Color.Lerp(heatMapSetting.MinColor, heatMapSetting.MaxColor, value);
                                float emission = Mathf.Lerp(0f, emissonIntensity, value);
                                HLSLRendererDictionary.MatrixInfo matrixInfo = HlslRenderer.DrawMeshInstance(worldPos, baseColor, emission, meshSize);

                                HeatMapItemHLSL heatMapItemHlsl = new HeatMapItemHLSL(matrixInfo, _hlslRenderer);
                                heatMapItemHlsl.SetHeatMapSetting(heatMapSetting);
                                _heatMapMatrixList.Add(heatMapItemHlsl);
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
                _isGenerateFinished = true;
                _hlslRenderer.isGenearteComplete = true;
                Debug.Log($"產生HLSL熱雲圖...Done!", this, EmojiEnum.Done);
            }
        }

        #region Variables

        [SerializeField] HeatMapSetting heatMapSetting;

        [Header(">>> 尺吋")] [SerializeField] float meshSize = 1f;
        [Header(">>> 間距")] [SerializeField] float spacing = 0.5f; // 間隔大小（格子大小）

        [Header(">>> 發光程度(會影響透明度)")] [SerializeField]
        private float emissonIntensity = 0.5f;

        [Header("運算批次處理")] [SerializeField] int batchSize = 1000;

        private Coroutine _coroutine;
        private HLSLRendererDictionary HlslRenderer => _hlslRenderer ??= GetComponent<HLSLRendererDictionary>();
        [NonSerialized] private HLSLRendererDictionary _hlslRenderer;
        private BoxCollider BoxColliderTarget => _boxColliderTarget ??= GetComponent<BoxCollider>();
        [NonSerialized] private BoxCollider _boxColliderTarget;

        /// 熱雲物件資訊
        private List<HeatMapItemHLSL> _heatMapMatrixList = new();

        private bool _isGenerateFinished = true;

        #endregion
    }
}