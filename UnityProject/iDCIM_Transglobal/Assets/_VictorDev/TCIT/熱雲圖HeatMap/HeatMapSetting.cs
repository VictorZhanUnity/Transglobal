using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class HeatMapSetting : MonoBehaviour
{
    /// 設定點位與值
    public void SetItemPoint(HeatMapPoint targetPoint)
    {
        //找出在範圍內的Item
        List<Transform> heatMapItemInRange = ObjectPlacer.ObjectList
            .Where(target => Vector3.Distance(target.position, targetPoint.Pos) < radiusRange)
            .OrderBy(target => Vector3.Distance(target.position, targetPoint.Pos)).ToList();
        
        targetPoint.HeatMapItemInRange = heatMapItemInRange;
        
        //算出pos與每個Item之間的Distance
        List<float> distances = heatMapItemInRange.Select(target => Vector3.Distance(target.position, targetPoint.Pos)).ToList();
        for (int i = 0; i < heatMapItemInRange.Count; ++i)
        {
            Color color = Color.Lerp(minColor, maxColor, (targetPoint.Value * 1f / maxValue) * (radiusRange-distances[i]));
            IHeatMapFogItem targetPointItem = heatMapItemInRange[i].GetComponent<IHeatMapFogItem>();
            if(i==0) targetPointItem.SetValue(Mathf.Clamp(targetPoint.Value, minValue, maxValue));
            targetPointItem.SetColor(color);
        }

        Debug.Log($"目標點位:{heatMapItemInRange[0]}");
    }

    [Button]
    public void InitHeatMapFog() => ObjectPlacer.PlaceObjects();

    [Button]
    public void RemoveHeatMapFog() => ObjectPlacer.RemoveAllChildren();

    public void OnObjectPlacedHandler(Transform obj)
    {
        if (obj.TryGetComponent(out IHeatMapFogItem fogItem))
        {
            fogItem.SetColor(minColor);
        }
    }

    #region Variables

    [Header("[設定] - 點位影響範圍")] [SerializeField]
    private float radiusRange = 1;

    [Foldout("[設定] - 最大值、最小值與顏色")] public int minValue = 0, maxValue = 100;

    [Foldout("[設定] - 最大值、最小值與顏色")] [SerializeField]
    private Color minColor = new Color(0, 1, 0, 30 / 255f);

    [Foldout("[設定] - 最大值、最小值與顏色")] [SerializeField]
    private Color maxColor = new Color(1, 0, 0, 150 / 255f);

   
    
    private ObjectPlacerInBound ObjectPlacer => _objectPlacerInBound ??= GetComponent<ObjectPlacerInBound>();
    [NonSerialized] private ObjectPlacerInBound _objectPlacerInBound;

    #endregion

    #region 測試
    [Foldout("[測試] - 點位與值")] [SerializeField]
    private List<HeatMapPoint> testPoints;

    [Button]
    private void TestPointTarget() => testPoints.ForEach(target=>SetItemPoint(target));
    #endregion
}