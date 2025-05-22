using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class HeatMapSetting : MonoBehaviour
{
    public void SetItemPoint(Vector3 pos, int value)
    {
        value = Mathf.Clamp(value, minValue, maxValue);
        Color color = Color.Lerp(minColor, maxColor, value * 1f / maxValue);
        List<IHeatMapFogItem> closetItem = ObjectPlacer.ObjectList
            .Where(t => Vector3.Distance(t.position, pos) < radiusRange)
            .OrderBy(target => Vector3.Distance(target.position, target.transform.position)).Select(obj=>obj.GetComponent<IHeatMapFogItem>())
            .ToList();

        IHeatMapFogItem targetPointItem = closetItem.First();
        targetPointItem.SetValue(value);
        targetPointItem.SetColor(color);
    }

    [Button]
    public void PlaceObjects() => ObjectPlacer.PlaceObjects();

    [Button]
    public void RemoveAllChildren() => ObjectPlacer.RemoveAllChildren();

    public void OnObjectPlacedHandler(Transform obj)
    {
        if (obj.TryGetComponent(out IHeatMapFogItem fogItem))
        {
            fogItem.SetColor(minColor);
        }
    }

    #region Variables

    [Header("[設定] - 影響範圍值")] [SerializeField]
    private float radiusRange = 1;

    [Foldout("[設定] - 最大值、最小值與顏色")] public int minValue = 0, maxValue = 100;

    [Foldout("[設定] - 最大值、最小值與顏色")] [SerializeField]
    private Color minColor = new Color(0, 1, 0, 30 / 255f);

    [Foldout("[設定] - 最大值、最小值與顏色")] [SerializeField]
    private Color maxColor = new Color(1, 0, 0, 150 / 255f);

    #region 測試
    [Foldout("[測試] - 點位與值")] [SerializeField]
    private Transform testPointTarget;
    [Foldout("[測試] - 點位與值")] [SerializeField]
    private int testValue;

    [Button]
    private void TestPointTarget() => SetItemPoint(testPointTarget.position, testValue);
    #endregion
   
    
    private ObjectPlacerInBound ObjectPlacer => _objectPlacerInBound ??= GetComponent<ObjectPlacerInBound>();
    [NonSerialized] private ObjectPlacerInBound _objectPlacerInBound;

    #endregion
}