using System;
using UnityEngine;
using VictorDev.ShaderUtils;
using VictorDev.HeatMapUtiils;
using Debug = VictorDev.Common.Debug;

/// 雲物件資訊 (HLSL)
public class HeatMapItemHLSL : IHeatMapItem
{
    public float Value{ get; private set; }
    public Vector3 Position => _matrixInfo.GetPosition();

    private HeatMapSetting _heatMapSetting;
    private HLSLRendererDictionary.MatrixInfo _matrixInfo;
    private HLSLRendererDictionary _hLslRenderer;

    
    public HeatMapItemHLSL(HLSLRendererDictionary.MatrixInfo matrixInfo, HLSLRendererDictionary hlslRenderer)
    {
        _matrixInfo = matrixInfo;
        _hLslRenderer = hlslRenderer;
    } 

    public void SetHeatMapSetting(HeatMapSetting heatMapSetting) => _heatMapSetting = heatMapSetting;

    public void SetWeightValue(int baseValue, float weightValue)
    {
        if (IsHeatMapPoint) return; //若為目標點位則跳過
        Value = baseValue * (1 - weightValue/_heatMapSetting.RadiusRange);
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
        ratio *= 2; // 刻意加強比例
        Color color = Color.Lerp(_heatMapSetting.MinColor, _heatMapSetting.MaxColor, ratio);
        color.a = ratio > 0.5f ? color.a : 1/255f; // 刻意設定alpha規則
        float emission = Mathf.Lerp(0, _heatMapSetting.MaxEmissionIntensity, ratio);
        emission = 0; // 刻意設定emission
        _hLslRenderer.SetMeshInstanceColor(_matrixInfo, color, emission);
    }
   
    public bool IsShowValue { get; set; }
    public bool IsHeatMapPoint { get; set; }
    
    
    public override string ToString() => $"HeatMapMatrix雲物件：{Position} => Value:{Value}";
}