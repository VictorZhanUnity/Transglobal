using System;
using TMPro;
using UnityEngine;

namespace VictorDev.TCIT.HeatMapUtiils
{
    public class HeatMapFogItem_VFX : MonoBehaviour, IHeatMapFogItem
    {
        /// 計算權重並設定顏色
        private void UpdateUI()
        {
            if (IsShowValue) TxtValue.SetText(_adjustValue.ToString("0.###"));
            float ratio = _adjustValue / _heatMapSetting.MaxValue;
            Color color = Color.Lerp(_heatMapSetting.MinColor, _heatMapSetting.MaxColor, ratio);
            SetColor(color);
        }

        /// 設定顏色
        private void SetColor(Color color)
        {
        }

        public void SetWeightValue(int baseValue, float weightValue)
        {
            if (IsHeatMapPoint) return; //若為目標點位則跳過
            _adjustValue += baseValue * (_heatMapSetting.RadiusRange - weightValue);
            SetValue(_adjustValue);
        }

        public void SetValue(float value)
        {
            _adjustValue = Mathf.Clamp(value, _heatMapSetting.MinValue, _heatMapSetting.MaxValue);
            UpdateUI();
        }

        public void Initialized(HeatMapSetting heatMapSetting)
        {
            _heatMapSetting = heatMapSetting;
            SetColor(_heatMapSetting.MinColor);
        }

        #region Variables

        /// 數值(目標點位值 or 權重值加總)
        private float _adjustValue;

        public bool IsShowValue
        {
            get => TxtValue.gameObject.activeSelf;
            set => TxtValue.gameObject.SetActive(value);
        }

        public bool IsHeatMapPoint { get; set; }

        private HeatMapSetting _heatMapSetting;
        private TextMeshPro TxtValue => _txtValue ??= transform.GetComponentInChildren<TextMeshPro>(true);
        [NonSerialized] private TextMeshPro _txtValue;

        #endregion
    }
}