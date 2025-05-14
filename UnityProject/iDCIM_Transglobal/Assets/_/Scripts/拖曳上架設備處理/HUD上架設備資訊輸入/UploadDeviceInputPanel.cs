using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDEV.Revit;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VictorDev.TextUtils;

public class UploadDeviceInputPanel : MonoBehaviour
{
    [Header("[Event] - 上架設備成功時Invoke")]
    public UnityEvent<DeviceModelDataExtended> onUploadSuccess = new();
    
    /// 接收點擊上架庫存設備的資訊
    public void ReceiveUploadData(DeviceModelDataExtended stockDevice, RackModelDataExtended rack,
        List<int> occupyULevel)
    {
        _stockDevice = stockDevice;
        _rack = rack;
        _occupyULevel = occupyULevel;
        UpdateUI();
    }

    private void UpdateUI()
    {
        TxtOccupyULevel.SetText($"U{_occupyULevel.Min()} ~ U{_occupyULevel.Max()}");
        TxtDeviceType.SetText(_stockDevice.DeviceType);
        TxtWatt.SetText(_stockDevice.information.watt.ToString());
        TxtWeight.SetText(_stockDevice.information.weight.ToString());
        TxtHeightU.SetText(_stockDevice.information.heightU.ToString());
        gameObject.SetActive(true);
    }   

    /// 進行上架，儲存至WebAPI
    public void UploadDevice()
    {
        //WebAPI記錄

        OnUploadDeviceSuccess();
    }

    private void OnUploadDeviceSuccess()
    {
        List<RackSpaceDisplayer> needToRemove = _rack.AvailableUDisplayer.Where(displayer=> _occupyULevel.Contains(displayer.ULevel)).ToList();
        needToRemove.ForEach(target=>
        {
            target.IsPinULocationVisible = false;
            target.HideULevel();
        });
        _rack.AvailableUDisplayer = _rack.AvailableUDisplayer.Except(needToRemove).ToList();
        onUploadSuccess?.Invoke(_stockDevice);
        CancelUpload();
    }

    /// 取消上架
    public void CancelUpload()
    {
        gameObject.SetActive(false);
        InputDeviceName.text = string.Empty;
        InputDeviceIP.text = string.Empty;
        InputDeviceDescription.text = string.Empty;
        ButtonUpload.interactable = false;

        _stockDevice = null;
        _rack = null;
        _occupyULevel = null;
    }

    #region Initialize
    private void Awake() => gameObject.SetActive(false);
    private void OnEnable()
    {
        TextHelper.EventCheckIsInputHaveValue(
            new List<TMP_InputField>() { InputDeviceName, InputDeviceIP, InputDeviceDescription }
            , (isOn) => ButtonUpload.interactable = isOn);
        ButtonUpload.onClick.AddListener(UploadDevice);
    }

    private void OnDisable()
    {
        InputDeviceName.onValueChanged.RemoveAllListeners();
        InputDeviceIP.onValueChanged.RemoveAllListeners();
        InputDeviceDescription.onValueChanged.RemoveAllListeners();
        ButtonUpload.onClick.RemoveListener(UploadDevice);
        CancelUpload();
    }

    #endregion

    #region Variables

    private DeviceModelDataExtended _stockDevice;
    private RackModelDataExtended _rack;
    private List<int> _occupyULevel;

    private Transform Container => _container ??= transform.Find("Container");
    [NonSerialized]
    private Transform _container;

    private TextMeshProUGUI TxtOccupyULevel =>
        _txtOccupyULevel ??= Container.Find("TxtOccupyULevel").GetComponent<TextMeshProUGUI>();
    [NonSerialized]
    private TextMeshProUGUI _txtOccupyULevel;

    private Transform VLayout => _vLayout ??= Container.Find("VLayout");
    [NonSerialized]
    private Transform _vLayout;

    private TMP_InputField InputDeviceName => _inputDeviceName ??=
        VLayout.Find("Input設備名稱").Find("InputDeviceName").GetComponent<TMP_InputField>();

    [NonSerialized]
    private TMP_InputField _inputDeviceName;

    private TextMeshProUGUI TxtDeviceType =>
        _txtDeviceType ??= VLayout.Find("TxtDeviceType").GetComponent<TextMeshProUGUI>();

    [NonSerialized]
    private TextMeshProUGUI _txtDeviceType;

    private TextMeshProUGUI TxtWatt =>
        _txtWatt ??= VLayout.Find("Title資源消耗").Find("TxtWatt").GetComponent<TextMeshProUGUI>();

    [NonSerialized]
    private TextMeshProUGUI _txtWatt;

    private TextMeshProUGUI TxtWeight =>
        _txtWeight ??= VLayout.Find("Title資源消耗").Find("TxtWeight").GetComponent<TextMeshProUGUI>();

    [NonSerialized]
    private TextMeshProUGUI _txtWeight;

    private TextMeshProUGUI TxtHeightU =>
        _txtHeightU ??= VLayout.Find("Title資源消耗").Find("TxtHeightU").GetComponent<TextMeshProUGUI>();

    [NonSerialized]
    private TextMeshProUGUI _txtHeightU;

    private TMP_InputField InputDeviceIP => _inputDeviceIP ??=
        VLayout.Find("IP位置").Find("InputDeviceIP").GetComponent<TMP_InputField>();

    [NonSerialized]
    private TMP_InputField _inputDeviceIP;

    private TMP_InputField InputDeviceDescription => _inputDeviceDescription ??=
        VLayout.Find("備註").Find("InputDeviceDescription").GetComponent<TMP_InputField>();

    [NonSerialized]
    private TMP_InputField _inputDeviceDescription;

    private Button ButtonUpload =>
        _buttonUpload ??= Container.Find("BottomHLayout").Find("ButtonUpload").GetComponent<Button>();

    [NonSerialized]
    private Button _buttonUpload;

    #endregion
}