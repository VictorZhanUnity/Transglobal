using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using VictorDev.Common;

/// 處理機櫃與其設備模型之Paret架構
/// <param> 利用Collider是否交互重疊，判斷是否為該機櫃的設備</param>
public class RackAndDeviceParentHandler : MonoBehaviour
{
    [Header("[設定] - 機櫃模型關鍵字")]
    [SerializeField] private List<string> rackKeywords = new (){"RACK", "ATEN"};
    
    [Button]
    private void SetRackAndModelsParent()
    {
        Racks.ForEach(targetRack =>
        {
            List<GameObject> modelInBound = ObjectHelper.GetModelInBound(targetRack, ModelsWithNotRack);
            modelInBound.ForEach(obj => obj.transform.parent = targetRack.transform);
        });
    }
    [Button]
    private void ClearDataList()
    {
        _racks.Clear();
        _modelsWithNotRack.Clear();
    }
    
    #region Variables
    /// 場景上所有機櫃
    private List<GameObject> Racks
    {
        get
        {
            if (_racks.Count == 0)
            {
                Collider[] colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None); // 找到所有Collider
                _racks = colliders.Where(col => rackKeywords.Any(word=> col.name.Contains(word, StringComparison.OrdinalIgnoreCase)))
                    .Select(col => col.gameObject).ToList();
            }
            return _racks;
        }
    }
    private List<GameObject> _racks = new ();

    /// 場景上所有非機櫃模型
    private List<GameObject> ModelsWithNotRack
    {
        get
        {
            if (_modelsWithNotRack.Count == 0)
            {
                Collider[] colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None); // 找到所有Collider
                _modelsWithNotRack = colliders.Where(col => rackKeywords.Any(word=> col.name.Contains(word, StringComparison.OrdinalIgnoreCase)) == false)
                    .Select(col => col.gameObject).ToList();
            }

            return _modelsWithNotRack;
        }
    }
    private List<GameObject> _modelsWithNotRack = new ();
    #endregion
}