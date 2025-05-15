using System;
using _VictorDEV.Revit;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UploadDeviceMessage : MonoBehaviour
{
    public UnityEvent<DeviceModelDataExtended> onUploadSuccess = new();
    
    public void ReceiveDeviceData(DeviceModelDataExtended deivceData)
    {
        _deviceData = deivceData;
        UpdateUI();
    }

    private void UpdateUI()
    {
        TxtDeviceName.SetText(_deviceData.DeviceName);
        gameObject.SetActive(true);
        onUploadSuccess?.Invoke(_deviceData);
        Invoke("Close", 3);
    }

    private void Close()
    {
        transform.GetChild(0).GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(() => gameObject.SetActive(false));
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private DeviceModelDataExtended _deviceData;

    private TextMeshProUGUI TxtDeviceName => _txtDeviceName ??= transform.GetChild(0).Find("TxtDeviceName").GetComponent<TextMeshProUGUI>();
    [NonSerialized]
    private TextMeshProUGUI _txtDeviceName;
}