using System;
using TMPro;
using UnityEngine;

public class HeatMapFogItem_Particle : MonoBehaviour, IHeatMapFogItem
{
    /// 計算權重並設定顏色
    private void UpdateUI()
    {
        if(IsShowValue)TxtValue.SetText(_adjustValue.ToString("0.###"));
        float ratio = _adjustValue / _heatMapSetting.MaxValue;
        Color color = Color.Lerp(_heatMapSetting.MinColor, _heatMapSetting.MaxColor, ratio);
        SetColor(color);
    }
    
    /// 設定顏色
    private void SetColor(Color color)
    {
        var mainModule = ParticleTarget.main;
        mainModule.startColor = color;
        ParticleTarget.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        ParticleTarget.Play();
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
    private ParticleSystem ParticleTarget => _particleTarget ??= GetComponent<ParticleSystem>();
    [NonSerialized] private ParticleSystem _particleTarget;
    private TextMeshPro TxtValue => _txtValue ??= transform.GetComponentInChildren<TextMeshPro>(true);
    [NonSerialized] private TextMeshPro _txtValue;
    #endregion
}