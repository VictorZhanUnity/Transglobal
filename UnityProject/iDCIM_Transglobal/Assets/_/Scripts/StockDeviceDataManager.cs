using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDEV.Revit;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using VictorDev.Common;
using VictorDev.FileUtils;
using VictorDev.Parser;
using Debug = VictorDev.Common.Debug;

public class StockDeviceDataManager : MonoBehaviour
{
    /// 讀取Resource資料夾裡的JSON檔
    [Button]
    public void LoadResourceFile()
    {
        rackModelList.Clear();
        ResourceFileLoader.LoadJsonFile(fileName, OnSuccessHandler);
    }

    private void OnSuccessHandler(string jsonString)
    {
        rackModelList = JsonConvert.DeserializeObject<List<RackModelDataExtended>>(jsonString);
        Debug.Log($"JsonString: {jsonString}");
        InvokeEvent();
    }

    /// 將所有機櫃裡的設備，依照型號群組化，列印出來
    [Button]
    private void GroupDeviceByDeviceType()
    {
        deviceList = rackModelList.SelectMany(rack => rack.Containers).GroupBy(device => device.DeviceType)
            .Select(g=> g.FirstOrDefault()).ToList();
        string printString = JsonConvert.SerializeObject(deviceList);
        Debug.Log($"GroupByDeviceType:\n{JsonHelper.PrintJSONFormatting(printString)}", this, EmojiEnum.DataBox);
    }
    
    public void InvokeEvent()
    {
        receivers.OfType<IReceiverStockDeviceModelDataExtended>().ToList().ForEach(target=> target.ReceiverData(deviceList));
    }

    private void Start()
    {
        InvokeEvent();
    }

    private void OnValidate() => receivers = ObjectHelper.CheckTypoOfList<IReceiverStockDeviceModelDataExtended>(receivers);

    #region Varialbes

    [Header(">>> 資料接收器 - IReceiverStockDeviceModelDataExtended")]
    [SerializeField] private List<MonoBehaviour> receivers;

    [Header("[設定] - 讀取檔名")]
    [SerializeField] private string fileName = "StockDeviceJsonData";
    [SerializeField] private List<DeviceModelDataExtended> deviceList;
    #endregion

    public interface IReceiverStockDeviceModelDataExtended
    {
        /// 接收所有庫存設備資料
        void ReceiverData(List<DeviceModelDataExtended> data);
    }
    
    public List<RackModelDataExtended> rackModelList;

}