using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDEV.Revit;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = VictorDev.Common.Debug;

/// 庫存設備列表
public class StockDeviceList : MonoBehaviour, StockDeviceDataManager.IReceiverStockDeviceModelDataExtended
{
    [Header("[Event] - 點選ListItem時Invoke {DeviceModelDataExtended}")]
    public UnityEvent<DeviceModelDataExtended> onSelectedItem = new();
    
    [Header("[Event] - 無點選ListItem時Invoke")]
    public UnityEvent onNoSelectedItem = new();

    public void ReceiverData(List<DeviceModelDataExtended> data)
    {
        _stockDeviceDataList = data.OrderBy(deviceData => deviceData.DeviceKind)
            .ThenBy(deviceData => deviceData.DeviceName).ToList();
        
        OnToggleChangeHandler(true, "", ServerDeviceDataList);

        InitToggleListener();
    }

    private void OnToggleChangeHandler(bool isOn, string label, List<DeviceModelDataExtended> list)
    {
        if (isOn == false) return;
        _currentLabel = label;
        _filterList = list;
        UpdateUI();
    }

    /// 上傳成功後移除庫存資料項
    public void RemoveStockDevice(DeviceModelDataExtended stockDevice)
    {
        _filterList.Remove(stockDevice);
        RefreshScrollRect();
    }

    /// 取消選擇庫存設備
    public void CancelSelectDevice()
    {
        if (_currentSelectedListItem != null)
        {
          _currentSelectedListItem.IsOn = false;
          _currentSelectedListItem = null;
        }
    }

    private void UpdateUI()
    {
        RefreshScrollRect();
    }

    /// 重整 ScrollRect列表
    private void RefreshScrollRect()
    {
        ClearScrollRect();
        
        _filterList.ForEach(deviceData =>
        {
            StockDeviceListItem item = Instantiate(listItemPrefab, ScrollRectInstance.content);
            item.ReceiveData(deviceData);
            item.ToggleInstance.group = ItemToggleGroup;
            item.ToggleInstance.onValueChanged.AddListener(_=>
                OnClickListItemHandler(item));
            ItemList.Add(item);
        });
        ScrollRectInstance.verticalNormalizedPosition = 1;
    }

    private void OnClickListItemHandler(StockDeviceListItem listItem)
    {
        _currentSelectedListItem = listItem;
        selectedDeviceData = listItem.ToggleInstance.isOn ? listItem.DeviceModelData : null;

        bool isHaveSelected = ItemList.Any(item=> item.ToggleInstance.isOn);
        if (isHaveSelected) onSelectedItem?.Invoke(listItem.DeviceModelData);
        else onNoSelectedItem?.Invoke();

        if (isHaveSelected)
        {
            Debug.Log($"devicePath: {listItem.DeviceModelData.devicePath}");
            Debug.Log($"Model.name: {listItem.DeviceModelData.Model.name}");
        }
    }
    
    public DeviceModelDataExtended selectedDeviceData;

    /// 清空ScrollRect列表
    private void ClearScrollRect()
    {
        foreach (Transform child in ScrollRectInstance.content)
        {
            Destroy(child.gameObject);
        }
        ItemList.Clear();
    }

    #region Initialize

    private void InitToggleListener()
    {
        ToggleServer.gameObject.SetActive(ServerDeviceDataList.Count > 0);
        ToggleRouter.gameObject.SetActive(RouterDeviceDataList.Count > 0);
        ToggleSwitch.gameObject.SetActive(SwitchDeviceDataList.Count > 0);
        if (ToggleServer.gameObject.activeSelf)
            ToggleServer.onValueChanged.AddListener((isOn) =>
                OnToggleChangeHandler(isOn, _labels[1], ServerDeviceDataList));
        if (ToggleRouter.gameObject.activeSelf)
            ToggleRouter.onValueChanged.AddListener((isOn) =>
                OnToggleChangeHandler(isOn, _labels[2], RouterDeviceDataList));
        if (ToggleSwitch.gameObject.activeSelf)
            ToggleSwitch.onValueChanged.AddListener((isOn) =>
                OnToggleChangeHandler(isOn, _labels[3], SwitchDeviceDataList));
    }

    private void OnDisable()
    {
        ToggleServer.onValueChanged.RemoveListener((isOn) =>
            OnToggleChangeHandler(isOn, _labels[1], ServerDeviceDataList));
        ToggleRouter.onValueChanged.RemoveListener((isOn) =>
            OnToggleChangeHandler(isOn, _labels[2], RouterDeviceDataList));
        ToggleSwitch.onValueChanged.RemoveListener((isOn) =>
            OnToggleChangeHandler(isOn, _labels[3], SwitchDeviceDataList));
        ClearScrollRect();
    }

    #endregion

    #region Variables

    private StockDeviceListItem _currentSelectedListItem;
    
    private List<StockDeviceListItem> ItemList { get; set; } = new ();

    /// 庫存設備資料
    private List<DeviceModelDataExtended> _stockDeviceDataList;

    /// 過濾後資料
    private List<DeviceModelDataExtended> _filterList = new List<DeviceModelDataExtended>();

    [Header("[Prefab] - 庫存設備列表Item")] [SerializeField]
    private StockDeviceListItem listItemPrefab;

    private List<DeviceModelDataExtended> ServerDeviceDataList => GetDeviceDataByDeviceKind(EnumReviteModelKind.Server);
    private List<DeviceModelDataExtended> RouterDeviceDataList => GetDeviceDataByDeviceKind(EnumReviteModelKind.Router);
    private List<DeviceModelDataExtended> SwitchDeviceDataList => GetDeviceDataByDeviceKind(EnumReviteModelKind.Switch);

    private List<DeviceModelDataExtended> GetDeviceDataByDeviceKind(EnumReviteModelKind deviceKind) =>
        _stockDeviceDataList
            .Where(device => device.DeviceKind.Equals(deviceKind)).OrderBy(device => device.DeviceType).ToList();

    private readonly List<string> _labels = new() { "所有設備", "Server", "Router", "Switch" };
    private string _currentLabel;

    
    /// Container內容
    private ToggleGroup ItemToggleGroup => _toggleGroup ??= GetComponent<ToggleGroup>();
    [NonSerialized]
    private ToggleGroup _toggleGroup;
    private Transform Container => _container ??= transform.Find("Container");
    [NonSerialized]
    private Transform _container;
    private TMP_InputField SearchBar => _searchBar ??= Container.Find("搜尋框 SearchBar").GetComponent<TMP_InputField>();
    [NonSerialized]
    private TMP_InputField _searchBar;
    private ScrollRect ScrollRectInstance => _scrollRect ??= Container.Find("ScrollRect").GetComponent<ScrollRect>();
    [NonSerialized]
    private ScrollRect _scrollRect;

    /// Toggle設備類型
    private Transform ToggleContainer => _toggleContainer ??= transform.Find("設備類型").Find("Container");
    [NonSerialized]
    private Transform _toggleContainer;

    private Toggle ToggleServer => _toggleServer ??= ToggleContainer.GetChild(0).GetComponent<Toggle>();
    [NonSerialized]
    private Toggle _toggleServer;

    private Toggle ToggleRouter => _toggleRouter ??= ToggleContainer.GetChild(1).GetComponent<Toggle>();
    [NonSerialized]
    private Toggle _toggleRouter;

    private Toggle ToggleSwitch => _toggleSwitch ??= ToggleContainer.GetChild(2).GetComponent<Toggle>();
    [NonSerialized]
    private Toggle _toggleSwitch;

    #endregion

   
}