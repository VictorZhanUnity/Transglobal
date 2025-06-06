using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.HeatMapUtiils
{
    //熱雲圖值點位資訊
    public class HeatMapPoint : MonoBehaviour
    {
        /// 設置在範圍內的雲物件HeatMapItem，每次設置皆會重置先前儲存的HeatMapItem
        public void SetHeatMapItemInRange(List<HeatMapItemHLSL> heatMapMatrices)
        {
            ResetHeatMapItemValue();
            _heatMapItemInRange = heatMapMatrices;
            UpdateHeatMapItem();

            Debug.Log($"目標點位:{_heatMapItemInRange[0]}");
        }

        public void ClearHeatMapItemInRange() => _heatMapItemInRange?.Clear();

        /// 設定雲物件的點位值/權重值
        private void UpdateHeatMapItem(bool isReset = false)
        {
            if(_coroutine != null) StopCoroutine(Func());
            _coroutine = StartCoroutine(Func());
            return;

            IEnumerator Func()
            {
                int counter = 0;
                foreach (var heatMapItem in _heatMapItemInRange)
                {
                    if (isReset) heatMapItem.SetValue(0);

                    float distance = Vector3.Distance(transform.position, heatMapItem.Position);
                    
                    if (heatMapItem == _heatMapItemInRange.First() )
                    {
                        heatMapItem.SetValue(value); //設定值給最靠近的雲物件
                        heatMapItem.IsHeatMapPoint = true;
                    }
                    else
                        heatMapItem.SetWeightValue(value,distance); //以距離為權重值

                    yield return new WaitForEndOfFrame();
                }
            }
        }

        /// 重設週圍HeatMapItem的數值為0
        private void ResetHeatMapItemValue() => _heatMapItemInRange?.ForEach(target => target.SetValue(0));

        private void OnValidate() => Txt.SetText(value.ToString());

        #region Variables

        [Range(0, 100)] [SerializeField] int value;

        /// 點位值
        public int Value
        {
            set
            {
                this.value = value;
                OnValidate();
                UpdateHeatMapItem(true);
            }
        }

        private Coroutine _coroutine;
        
        /// 在範圍內的雲物件HeatMapItem
        [NonSerialized] private List<HeatMapItemHLSL> _heatMapItemInRange = new();

        private TextMeshPro Txt => _txt ??= transform.GetComponentInChildren<TextMeshPro>();
        [NonSerialized] private TextMeshPro _txt;

        #endregion
    }
}