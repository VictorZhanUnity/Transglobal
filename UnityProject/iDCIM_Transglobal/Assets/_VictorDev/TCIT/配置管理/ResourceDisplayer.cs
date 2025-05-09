using System;
using _VictorDEV.Revit;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class ResourceDisplayer : MonoBehaviour
{
    [NonSerialized]
    private DeviceModelDataExtended _selectedDeviceModelData;

    [Foldout("[設定]")] public TextMeshProUGUI txtPreUseWatt, txtPreUseWeight, txtPreUseHeightU;

    public void ReceiverData(DeviceModelDataExtended data)
    {
        _selectedDeviceModelData = data;
        UpdateUI();
    }

    private void UpdateUI()
    {
        bool isVisible = _selectedDeviceModelData != null;
        txtPreUseWatt.gameObject.SetActive(isVisible);   
        txtPreUseWeight.gameObject.SetActive(isVisible);   
        txtPreUseHeightU.gameObject.SetActive(isVisible);
        
        if (_selectedDeviceModelData != null)
        {
            txtPreUseWatt.SetText("-" + _selectedDeviceModelData.information.watt.ToString("N0"));
            txtPreUseWeight.SetText("-" + _selectedDeviceModelData.information.weight.ToString("N0"));
            txtPreUseHeightU.SetText("-" + _selectedDeviceModelData.information.heightU.ToString());
        }
    }

    public void Cancel()
    {
        _selectedDeviceModelData = null;
        UpdateUI();
    }

    private void Start() => UpdateUI();
}