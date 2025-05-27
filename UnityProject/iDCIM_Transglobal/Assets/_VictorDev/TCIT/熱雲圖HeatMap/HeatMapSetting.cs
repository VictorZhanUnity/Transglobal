using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace VictorDev.TCIT.HeatMapUtiils
{
    public class HeatMapSetting : MonoBehaviour
    {
        /// 設定點位與值
        public void SetItemPoint(HeatMapPoint targetPoint)
        {
            //找出在範圍內的Item
            List<Transform> heatMapItemInRange = ObjectPlacer.ObjectList
                .Where(target => Vector3.Distance(target.position, targetPoint.transform.position) < radiusRange)
                .ToList();

            targetPoint.SetHeatMapItemInRange(heatMapItemInRange);
            Debug.Log($"目標點位:{heatMapItemInRange[0]}");
        }

        [Button]
        public void InitHeatMapFog() => ObjectPlacer.PlaceObjects();

        [Button]
        public void RemoveHeatMapFog() => ObjectPlacer.RemoveAllChildren();

        /// 初始化雲物件時
        public void OnObjectPlacedHandler(Transform obj)
        {
            if (obj.TryGetComponent(out IHeatMapFogItem fogItem))
            {
                fogItem.Initialized(this);
                fogItem.IsShowValue = isShowValueTxt;

                if (Application.isEditor)
                {
                    _heatMapItems.Add(fogItem);
                }
            }
        }

        private void OnValidate() => _heatMapItems.ForEach(item => item.IsShowValue = isShowValueTxt);

        #region Variables

        private readonly List<IHeatMapFogItem> _heatMapItems = new();

        [Header("[設定] - 點位影響範圍")] [SerializeField]
        private float radiusRange = 1;

        [Foldout("[設定] - 雲物件相關設定")] [SerializeField]
        private bool isShowValueTxt;

        [Foldout("[設定] - 雲物件相關設定")] [SerializeField]
        private int minValue = 0, maxValue = 100;

        [Foldout("[設定] - 雲物件相關設定")] [SerializeField]
        private Color minColor = new Color(0, 1, 0, 30 / 255f);

        [Foldout("[設定] - 雲物件相關設定")] [SerializeField]
        private Color maxColor = new Color(1, 0, 0, 150 / 255f);

        public float RadiusRange => radiusRange;
        public int MinValue => minValue;
        public int MaxValue => maxValue;
        public Color MinColor => minColor;
        public Color MaxColor => maxColor;

        private ObjectPlacerInBound ObjectPlacer => _objectPlacerInBound ??= GetComponent<ObjectPlacerInBound>();
        [NonSerialized] private ObjectPlacerInBound _objectPlacerInBound;

        #endregion

        #region 測試

        [Foldout("[測試] - 點位與值")] [SerializeField]
        private List<HeatMapPoint> testPoints;

        [Button]
        private void TestPointTarget() => testPoints.ForEach(target => SetItemPoint(target));

        #endregion
    }
}