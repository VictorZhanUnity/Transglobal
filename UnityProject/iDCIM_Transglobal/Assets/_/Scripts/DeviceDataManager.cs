using System.Collections.Generic;
using _VictorDEV.Revit;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using VictorDev.FileUtils;
using Debug = VictorDev.Common.Debug;

public class DeviceDataManager : MonoBehaviour
{
    [SerializeField] private string streamAssetFileUrl = "DeviceJsonData";

    private string Result;
 
    public List<RackModelDataExtended> rackModelList;
 
    [Button]
    public void LoadResourceFile()
    {
        rackModelList.Clear();
        ResourceFileLoader.LoadJsonFile(streamAssetFileUrl, onSuccessHandler);
    }

    private void onSuccessHandler(string jsonString)
    {
        rackModelList = JsonConvert.DeserializeObject<List<RackModelDataExtended>>(jsonString);
        Debug.Log($"Result: {Result}");
    }
}