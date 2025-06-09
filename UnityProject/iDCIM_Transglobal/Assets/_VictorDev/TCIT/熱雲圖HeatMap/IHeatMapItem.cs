
using UnityEngine;

namespace VictorDev.HeatMapUtiils
{
    public interface IHeatMapItem
    {
        /// 套用熱雲圖設定
        void SetHeatMapSetting(HeatMapSetting heatMapSetting);

        /// 設定所屬於哪些HeatMapPoint點位，用於計算權重
        void SetHeatMapPoint(HeatMapPoint heatMapPoint);
        
        /// 設定值(供目標點位使用)
        void SetValue(float value);

        /// 將所屬於HeatMapPoint點位的記錄清空，並設值為0
        void ResetPoint();
        
        /// 值
        float Value { get; }
        /// 座標
        Vector3 Position{ get; }

        /// 是否為數值點位(若是則無法使用SetWeightValue())
        bool IsHeatMapPoint { get; set; }
        
        /// 是否顯示值
        bool IsShowValue { get; set; }

    }
}