using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VictorDev.HeatMapUtiils;
using VictorDev.ShaderUtils;

/// 雲物件資訊 (HLSL)
public class HeatMapItemHLSL : IHeatMapItem
{
    public float Value { get; private set; }
    public Vector3 Position => _matrixInfo.GetPosition();

    private HeatMapSetting _heatMapSetting;
    private HLSLRenderer.MatrixInfo _matrixInfo;
    private HLSLRenderer _hLslRenderer;

    public HeatMapItemHLSL(HLSLRenderer.MatrixInfo matrixInfo, HLSLRenderer hlslRenderer)
    {
        _matrixInfo = matrixInfo;
        _hLslRenderer = hlslRenderer;
    }

    public void SetHeatMapSetting(HeatMapSetting heatMapSetting) => _heatMapSetting = heatMapSetting;

    private List<HeatMapPoint> _heatMapPoints = new ();
    
    public void SetHeatMapPoint(HeatMapPoint heatMapPoint)
    {
        if (_heatMapPoints.Contains(heatMapPoint) == false)
        {
            _heatMapPoints.Add(heatMapPoint);
        }

        // 依照所在的HeatMapPoint點位，計算加總的權重
        float weightValue = 0;
        var snapshot = _heatMapPoints.ToList();
        snapshot.ForEach(point =>
        {
            float distance = Vector3.Distance(point.transform.position, Position);
            weightValue += point.Value * (1-distance/_heatMapSetting.RadiusRange);
        });
        SetValue(weightValue);
    }

    public void SetValue(float value)
    {
        Value = Mathf.Clamp(value, _heatMapSetting.MinValue, _heatMapSetting.MaxValue);
        UpdateUI();
    }

    public void ResetPoint()
    {
        _heatMapPoints.Clear();
        SetValue(0);
    }

    private void UpdateUI()
    {
        float ratio = Mathf.Clamp01(Value / _heatMapSetting.MaxValue);
        ratio *= 2f; // 刻意加強比例
        Color color = Color.Lerp(_heatMapSetting.MinColor, _heatMapSetting.MaxColor, ratio);
        color.a = ratio > 0.4f ? color.a : 1 / 255f; // 刻意設定alpha規則
        float emission = Mathf.Lerp(0, _heatMapSetting.MaxEmissionIntensity, ratio);
        emission = 0; // 刻意設定emission
        _hLslRenderer.SetMeshInstanceColor(_matrixInfo, color, emission);
    }
    private float _baseValue;
    public bool IsShowValue { get; set; }
    public bool IsHeatMapPoint { get; set; }

    public override string ToString() => $"HeatMapMatrix雲物件：{Position} => Value:{Value}";
}