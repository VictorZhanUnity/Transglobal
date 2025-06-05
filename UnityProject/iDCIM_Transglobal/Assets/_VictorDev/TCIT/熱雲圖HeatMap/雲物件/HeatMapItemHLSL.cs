using System;
using UnityEngine;
using VictorDev.ShaderUtils;
using VictorDev.HeatMapUtiils;

/// 雲物件資訊 (HLSL)
public class HeatMapItemHLSL : IHeatMapItem
{
    public float Value{ get; private set; }
    public Vector3 Position => _matrix.GetPosition();

    private HeatMapSetting _heatMapSetting;
    private Matrix4x4 _matrix;
    private HLSLRenderer _hLslRenderer;

    
    public HeatMapItemHLSL(Matrix4x4 matrix, HLSLRenderer hlslRenderer)
    {
        _matrix = matrix;
        _hLslRenderer = hlslRenderer;
    } 

    public void SetHeatMapSetting(HeatMapSetting heatMapSetting) => _heatMapSetting = heatMapSetting;

    public void SetWeightValue(int baseValue, float weightValue)
    {
        if (IsHeatMapPoint) return; //若為目標點位則跳過

        float multiplier = 1.0f / _heatMapSetting.RadiusRange;
        Value = baseValue * (_heatMapSetting.RadiusRange - weightValue);
        SetValue(Value);
    }
    
    public void SetValue(float value)
    {
        Value = Mathf.Clamp(value, _heatMapSetting.MinValue, _heatMapSetting.MaxValue);
        UpdateUI();
    }

    private void UpdateUI()
    {
        float ratio = Mathf.Clamp01(Value / _heatMapSetting.MaxValue);
        Color color = Color.Lerp(_heatMapSetting.MinColor, _heatMapSetting.MaxColor, ratio);
        float emission = Mathf.Lerp(0, _heatMapSetting.MaxEmissionIntensity, ratio);
        emission = 0;
        _hLslRenderer.ChangeMeshInstanceColor(_matrix, color, emission);
        
        Debug.Log($"ratio: {ratio} / Value: {Value}");
    }
    
   
    public bool IsShowValue { get; set; }
    public bool IsHeatMapPoint { get; set; }
    
    
    public override string ToString() => $"HeatMapMatrix雲物件：{Position} => Value:{Value}";
}