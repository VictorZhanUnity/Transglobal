public interface IHeatMapFogItem
{
    /// 初始化，套用熱雲圖設定
    void Initialized(HeatMapSetting heatMapSetting);
 
    /// 設定權重值(供目標點位週圍使用)
    void SetWeightValue(int baseValue, float weightValue);

    /// 設定值(供目標點位使用)
    void SetValue(float value);
    
    /// 是否顯示值
    bool IsShowValue { get; set; }

    /// 是否為點位位置(若是則無法使用SetWeightValue())
    bool IsHeatMapPoint { get; set; }
}