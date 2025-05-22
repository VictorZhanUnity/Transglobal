using UnityEngine;

public interface IHeatMapFogItem
{
    /// 設置顏色
    void SetColor(Color color);
    /// 設置權重值
    void SetValue(int value);
    /// 調整權重值
    void AdjustValue(int value);
}
