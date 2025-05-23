using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

//熱力圖點位資訊
public class HeatMapPoint : MonoBehaviour
{
    [Range(0, 100)] [SerializeField] int value;

   
   
    public Vector3 Pos => transform.position;
    public int Value => value;

    
    /// 在範圍內的HeatMapItem
    public List<Transform> HeatMapItemInRange
    {
        get => _heatMapItemInRange;
        set
        {
            ResetHeatMapItemValue();
            _heatMapItemInRange = value;
        }
    }
    [NonSerialized] private List<Transform> _heatMapItemInRange;
    
    /// 重設熱力圖FogItem的Value權重為0
    private void ResetHeatMapItemValue()
        => HeatMapItemInRange?.Select(target => target.GetComponent<IHeatMapFogItem>()).ToList()
            .ForEach(target => target.SetValue(0));

    private void OnValidate() => transform.GetComponentInChildren<TextMeshPro>().SetText(value.ToString());
}