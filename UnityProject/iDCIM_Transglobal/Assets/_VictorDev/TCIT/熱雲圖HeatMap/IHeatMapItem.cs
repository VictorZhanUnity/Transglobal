
using UnityEngine;

namespace VictorDev.HeatMapUtiils
{
    public interface IHeatMapItem
    {
        /// 套用熱雲圖設定
        void SetHeatMapSetting(HeatMapSetting heatMapSetting);

        /// 設定值(供目標點位使用)
        void SetValue(float value);
        
        /// 設定權重值(供目標點位週圍使用)
        void SetWeightValue(int baseValue, float weightValue);
        
        /// 權重值
        float Value { get; }
        /// 座標
        Vector3 Position{ get; }

        /// 是否為數值點位(若是則無法使用SetWeightValue())
        bool IsHeatMapPoint { get; set; }
        
        /// 是否顯示值
        bool IsShowValue { get; set; }

    }
}