using System;
using System.Collections.Generic;
using _VictorDEV.Revit;
using UnityEngine;

/// 上傳至WebAPI產生Excel的JSON格式
/// <para>http://192.168.0.101:5054/swagger/index.html</para>
/// <para>/api/excelexport/prepare</para>
/// <para>/api/excelexport/start</para>
/// <para>/api/excelexport</para>
[Serializable]
public class ExcelRackRoomInfo : Dictionary<string, List<ExcelRackRoomInfo.RackInfo>>
{
    [Serializable]
    public class RackInfo
    {
        public string code = "Rack101";
        public string type = "ATEN-PCEIII-4211060-19空機櫃";
        public CapacityUsage capacityUsage = new();
        public List<Contains> contains = new();

        public void SetRackInfo(RackModelDataExtended rackData)
        {
            code = rackData.devicePath;
            type = rackData.DeviceType;
            capacityUsage = new CapacityUsage()
            {
                watt = rackData.information.watt_limit,
                weightCapacity = rackData.information.weight_limit,
                usedU = rackData.UsageOfHeightU,
                percentOfWT = Mathf.RoundToInt(rackData.UsagePercentOfWatt * 100),
                percentOfWC = Mathf.RoundToInt(rackData.UsagePercentOfWeight * 100),
                percentOfRU = Mathf.RoundToInt(rackData.UsagePercentOfHeightU * 100),
                radio = $"{rackData.UsageOfHeightU}/{rackData.information.heightU}",
            };
            rackData.Containers.ForEach(device =>
            {
                contains.Add(new Contains(){RU=device.rackLocation, info = device.DeviceType});
            });
        }
    }
    [Serializable]
    public class CapacityUsage
    {
        public int watt = 4800;
        public int weightCapacity = 320;

        public int usedU = 34;

        // 用電百分比
        public int percentOfWT = 32;

        // 負重百分比
        public int percentOfWC = 16;
        public int percentOfRU = 81;
        public string radio = "34/42";
    }
    [Serializable]
    public class Contains
    {
        /// RU位置
        public int RU = 42;
        public string info = "empty";
    }
}