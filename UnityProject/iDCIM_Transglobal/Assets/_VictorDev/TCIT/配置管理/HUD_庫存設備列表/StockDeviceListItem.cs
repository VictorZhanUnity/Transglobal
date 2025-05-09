using _VictorDEV.Revit;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StockDeviceListItem : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<DeviceModelDataExtended> onToggleOnEvent = new();
    
    public void ReceiveData(DeviceModelDataExtended data)
    {
        DeviceModelData = data;
        TxtDeviceName.SetText(DeviceModelData.DeviceType);
        TxtDeviceName_Selected.SetText(DeviceModelData.DeviceType);
        /*TxtWatt.SetText(data.information.watt.ToString());
        TxtWeight.SetText(data.information.weight.ToString());
        TxtHeightU.SetText(data.information.heightU.ToString());*/
    }

    #region Variables

    public bool IsOn
    {
        set => _toggleInstance.isOn = value;
    }
    
    public DeviceModelDataExtended DeviceModelData { get; private set; }

    public Toggle ToggleInstance => _toggleInstance ??= GetComponent<Toggle>();
    private Toggle _toggleInstance;

    private Transform Container => _container ??= transform.Find("Container");
    private Transform _container;

    private TextMeshProUGUI TxtDeviceName => _txtDeviceName ??= Container.Find("TxtDeviceName").GetComponent<TextMeshProUGUI>();
    private TextMeshProUGUI _txtDeviceName;
    
    private TextMeshProUGUI TxtDeviceName_Selected => _txtDeviceName_Selected ??= Container.Find("TxtDeviceName_Selected").GetComponent<TextMeshProUGUI>();
    private TextMeshProUGUI _txtDeviceName_Selected;
    
    private Image Icon => _icon ??= Container.Find("Icon").GetComponent<Image>();
    private Image _icon;
    
    
    private Transform HLayout => _hLayout ??= Container.Find("HLayout");
    private Transform _hLayout;
    
    private TextMeshProUGUI TxtWatt => _txtWatt ??= HLayout.Find("TxtWatt").GetComponent<TextMeshProUGUI>();
    private TextMeshProUGUI _txtWatt;
    private TextMeshProUGUI TxtWeight => _txtWeight ??= HLayout.Find("TxtWeight").GetComponent<TextMeshProUGUI>();
    private TextMeshProUGUI _txtWeight;
    private TextMeshProUGUI TxtHeightU => _txtHeightU ??= HLayout.Find("TxtHeightU").GetComponent<TextMeshProUGUI>();
    private TextMeshProUGUI _txtHeightU;
    #endregion
}