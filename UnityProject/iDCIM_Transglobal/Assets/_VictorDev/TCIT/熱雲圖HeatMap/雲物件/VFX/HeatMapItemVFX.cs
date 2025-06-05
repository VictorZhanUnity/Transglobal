using System;
using TMPro;
using UnityEngine;

namespace VictorDev.HeatMapUtiils
{
    public class HeatMapItemVFX : MonoBehaviour, IHeatMapItem
    {
        /// 計算權重並設定顏色
        private void UpdateUI()
        {
            if (IsShowValue) TxtValue.SetText(_adjustValue.ToString("0.###"));
            float ratio = _adjustValue / _heatMapSetting.MaxValue;
            ratio = Mathf.Clamp01(ratio);
            Color color = Color.Lerp(_heatMapSetting.MinColor, _heatMapSetting.MaxColor, ratio);

            if (ratio > 0.7f) color *= 1.5f;
            SetColor(color);

            float scaleValue = ratio > 0.7f ? 0.5f : 0.5f;

            //RendererTarget.material.SetFloat("_Opacity", ratio);
            RendererTarget.material.SetFloat("_Opacity", ratio*scaleValue);
        }

        /// 設定顏色
        private void SetColor(Color color)
        {
            RendererTarget.material.SetColor("_StartColor", color);
            RendererTarget.material.SetColor("_EndColor", color);
        }

        public void SetWeightValue(int baseValue, float weightValue)
        {
            if (IsHeatMapPoint) return; //若為目標點位則跳過
            _adjustValue += baseValue * (_heatMapSetting.RadiusRange - weightValue);
            SetValue(_adjustValue);
        }

        public float Value => _adjustValue;

        public void SetValue(float value)
        {
            _adjustValue = Mathf.Clamp(value, _heatMapSetting.MinValue, _heatMapSetting.MaxValue);
            UpdateUI();
        }

        public Vector3 Position => transform.position;

        public void SetHeatMapSetting(HeatMapSetting heatMapSetting)
        {
            _heatMapSetting = heatMapSetting;
            SetColor(_heatMapSetting.MinColor);
            RendererTarget.material.SetFloat("_Opacity", 0.05f);
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

        private Renderer RendererTarget => _rendererTarget ??= GetComponentInChildren<Renderer>();
        [NonSerialized] private Renderer _rendererTarget;

        #endregion
    }
}