using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using _VictorDEV.DateTimeUtils;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VictorDev.Net.WebAPI.TCIT;
using Debug = UnityEngine.Debug;

public class Demo_DeviceAssetToExcel : MonoBehaviour
{
    public TMP_InputField txtFilePath;
    
    private ExcelRackRoomInfo _rackRoomInfo;
    private string  _folder;
    private string _filePath;
    public Image image;
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
    }

    [Button("ToWebAPI_ExcelPrepare")]
    public void ToWebAPI_ExcelPrepare()
    {
        BuildExcelRoomData();
        WebApiExcelGenerator.ExcelPrepare(_rackRoomInfo, OnSuccess);
    }

    private void OnSuccess(long responseCode, byte[] excelData)
    {
        _folder = AppDomain.CurrentDomain.BaseDirectory;
        string fileName = $"ITAssetReport-{DateTime.Today.ToString(DateTimeHelper.FullDateFormat)}.xlsx";
        _filePath = Path.Combine(_folder, fileName);
        File.WriteAllBytes(_filePath, excelData);
        Debug.Log("Excel 檔案已儲存至: " + _filePath);

        txtFilePath.text = _filePath;
        try
        {
            // 開啟 Excel 檔案
            Process.Start(new ProcessStartInfo
            {
                FileName = _filePath,
                UseShellExecute = true // 讓作業系統選擇合適的應用程式來開啟
            });
        }catch (Exception e)
        {
            Debug.LogError("儲存或開啟 Excel 時發生錯誤: " + e.Message);
        }
    }

    public void CopyFilePathToClipboard()
    {
        GUIUtility.systemCopyBuffer = _folder;
        image.gameObject.SetActive(true);
    }
}