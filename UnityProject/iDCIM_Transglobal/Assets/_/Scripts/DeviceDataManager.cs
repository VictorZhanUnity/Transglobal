using System.Collections.Generic;
using System.Linq;
using _VictorDEV.Revit;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using VictorDev.Common;
using VictorDev.FileUtils;
using VictorDev.Parser;
using Debug = VictorDev.Common.Debug;

public class DeviceDataManager : MonoBehaviour
{
    private void Start()
    {
        LoadResourceFile();
    }

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
  
    private void InvokeEvent()
    {
        invokeRemainOfWatt?.Invoke((int)rackModelList.Sum(data => data.RemainOfWatt));
        invokeRemainOfWeight?.Invoke((int)rackModelList.Sum(data => data.RemainOfWeight));
        invokeRemainOfHeightU?.Invoke(rackModelList.Sum(data => data.RemainOfHeightU));
        invokeTotalWatt?.Invoke((int)rackModelList.Sum(data => data.information.watt_limit));
        invokeTotalWeight?.Invoke((int)rackModelList.Sum(data => data.information.weight_limit));
        invokeTotalHeightU?.Invoke(rackModelList.Sum(data => data.information.heightU));
        
        receivers.OfType<IReceiverRackModelDataExtended>().ToList().ForEach(target=> target.ReceiverData(rackModelList));
    }

    private void OnValidate() => receivers = ObjectHelper.CheckTypoOfList<IReceiverRackModelDataExtended>(receivers);

    #region Varialbes

    [Header(">>> 資料接收器 - IReceiverRackModelDataExtended")]
    [SerializeField] private List<MonoBehaviour> receivers;
    
    [Foldout("[Event]")] public UnityEvent<int> invokeRemainOfWatt = new();
    [Foldout("[Event]")] public UnityEvent<int> invokeRemainOfWeight = new();
    [Foldout("[Event]")] public UnityEvent<int> invokeRemainOfHeightU = new();
    [Foldout("[Event]")] public UnityEvent<int> invokeTotalWatt = new();
    [Foldout("[Event]")] public UnityEvent<int> invokeTotalWeight = new();
    [Foldout("[Event]")] public UnityEvent<int> invokeTotalHeightU = new();

    [Header("[設定] - 讀取檔名")]
    [SerializeField] private string fileName = "DeviceJsonData";
    public List<RackModelDataExtended> rackModelList;
    #endregion

    public interface IReceiverRackModelDataExtended
    {
        /// 接收所有機櫃與其設備資料
        void ReceiverData(List<RackModelDataExtended> data);
    }
}