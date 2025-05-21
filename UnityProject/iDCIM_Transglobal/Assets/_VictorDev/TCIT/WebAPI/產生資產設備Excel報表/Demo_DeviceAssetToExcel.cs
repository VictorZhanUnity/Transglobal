using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using _VictorDEV.DateTimeUtils;
using _VictorDEV.Revit;
using NaughtyAttributes;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VictorDev.FileUtils;
using VictorDev.Net.WebAPI.TCIT;
using Debug = UnityEngine.Debug;

public class Demo_DeviceAssetToExcel : MonoBehaviour
{
    [Foldout("組件控制")]
    public TMP_InputField txtFileFolder;
    [Foldout("組件控制")]
    public Image image;
    
    
    private ExcelRackRoomInfo _rackRoomInfo;
    [Button()]
    private void BuildExcelRoomData()
    {
        _rackRoomInfo = new ExcelRackRoomInfo();
        _rackRoomInfo["Room-A1"] = new List<ExcelRackRoomInfo.RackInfo>() { new() };
        _rackRoomInfo["Room-A2"] = new List<ExcelRackRoomInfo.RackInfo>() { new() };
        _rackRoomInfo["Room-A2"][0].contains = new List<ExcelRackRoomInfo.Contains>()
        {
            new() { RU = 42, info = "Brocade-CER2024C-4X-Router-1U" },
            new() { RU = 41, info = "Brocade-CER2024C-4X-Router-1U" },
            new() { RU = 40, info = "empty" },
            new() { RU = 39, info = "Brocade-CER2024C-4X-Router-1U" },
            new() { RU = 38, info = "Brocade-CER2024C-4X-Router-1U" },
            new() { RU = 38, info = "Brocade-CER2024C-4X-Router-1U" },
            new() { RU = 37, info = "Brocade-CER2024C-4X-Router-1U" },
            new() { RU = 36, info = "Brocade-CER2024C-4X-Router-1U" },
            new() { RU = 35, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 34, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 33, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 32, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 31, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 30, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 29, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 28, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 27, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 26, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 25, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 24, info = "Dell-PowerSwitchS系列-10GbE-Switch-6U" },
            new() { RU = 23, info = "empty" },
            new() { RU = 22, info = "Dell-PowerSwitchZ系列-Switch-2U" },
            new() { RU = 21, info = "Dell-PowerSwitchZ系列-Switch-2U" },
            new() { RU = 20, info = "Dell-PowerSwitchZ系列-Switch-2U" },
            new() { RU = 19, info = "Dell-PowerSwitchZ系列-Switch-2U" },
            new() { RU = 18, info = "Dell-PowerSwitchZ系列-Switch-2U" },
            new() { RU = 17, info = "Dell-PowerSwitchZ系列-Switch-2U" },
            new() { RU = 16, info = "empty" },
            new() { RU = 15, info = "empty" },
            new() { RU = 14, info = "empty" },
            new() { RU = 13, info = "empty" },
            new() { RU = 12, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 11, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 10, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 9, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 8, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 7, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 6, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 5, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 4, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 3, info = "Dell-PowerEdge系列-MX9116n-Server-2U" },
            new() { RU = 2, info = "empty" },
            new() { RU = 1, info = "empty" }
        };
        
        ToWebAPI_ExcelPrepare();
    }

    [Button]
    private void LoadResourceFile()
    {
        rackDataList.Clear();
        ResourceFileLoader.LoadJsonFile("DeviceJsonData", OnResourceLoadSuccess);
    }

    private void OnResourceLoadSuccess(string jsonString)
    {
        rackDataList = JsonConvert.DeserializeObject<List<RackModelDataExtended>>(jsonString);
        _rackRoomInfo = new ExcelRackRoomInfo();
        _rackRoomInfo["TransGlobal-A1"] = new List<ExcelRackRoomInfo.RackInfo>();
        rackDataList.ForEach(rackModelData =>
        {
            ExcelRackRoomInfo.RackInfo rackInfo = new ExcelRackRoomInfo.RackInfo();
            rackInfo.SetRackInfo(rackModelData);
            _rackRoomInfo["TransGlobal-A1"].Add(rackInfo);
        });
        ToWebAPI_ExcelPrepare();
    }
    
    public void ToWebAPI_ExcelPrepare() => WebApiExcelGenerator.ExcelPrepare(_rackRoomInfo, OnExcelPrepareSuccess);

    private void OnExcelPrepareSuccess(long responseCode, byte[] excelData)
    {
        string fileName = $"ITAssetReport-{DateTime.Today.ToString(DateTimeHelper.FullDateFormat)}.xlsx";
        txtFileFolder.text = FileHelper.DownloadHandlerFile(excelData, fileName)[0];
        CopyFilePathToClipboard();
    }

    private void CopyFilePathToClipboard()
    {
        GUIUtility.systemCopyBuffer = txtFileFolder.text;
        image.gameObject.SetActive(true);
    }

    public List<RackModelDataExtended> rackDataList;
}