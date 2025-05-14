using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VictorDev.Common;
using VictorDev.RevitUtils;
using Object = UnityEngine.Object;

namespace _VictorDEV.Revit
{
    /// Revit模型資料格式 - 機櫃 (擴增)
    [Serializable]
    public class RackModelDataExtended : RackModelData
    {
        /// 設備型號 {Brocade-7X-8-sim}
        public string DeviceType =>DevicePathSplit.Length > 6? DevicePathSplit[6].Split(":")[0].Trim() : "";

        /// 可用U空間物件List {RackSpaceDisplayer}
        public List<RackSpaceDisplayer> AvailableUDisplayer { get; set; } = new();

        /// 已使用的U層 (即時計算)
        public List<int> OccupyULevels => Containers
            .SelectMany(device => Enumerable.Range(device.rackLocation, device.information.heightU)).ToList();

        /// 剩餘空閒U層數 (即時計算) 
        public List<int> AvailableULevels =>
            Enumerable.Range(1, information.heightU).Except(OccupyULevels).ToList();

        /// 以目前指向的U層，找出放的下設備HeightU的合適U層
        public int GetSuitableStartULevel(DeviceModelDataExtended deviceData, int currentSelectedULevel)
        {
            //計算機櫃該RackSpacer空間是否放的下庫存設備
            int heightU = deviceData.information.heightU;
            int targetULevel = AvailableULevels.Select(u => u) //變成IEnumerable，使後續操作不會改變原數組
                .Reverse() //從後面開始算
                .FirstOrDefault(uLevel =>
                    Enumerable.Range(uLevel, heightU).All(AvailableULevels.Contains) //空閒U層放的下庫存設備
                    && Enumerable.Range(uLevel, heightU).Contains(currentSelectedULevel)); //空閒U層包含目前指向的U層
            return targetULevel;
        }
        
        public void ShowAvailableUDisplayer() =>
            AvailableUDisplayer.ForEach(displayer => displayer.gameObject.SetActive(true));

        public void HideAvailableUDisplayer() =>
            AvailableUDisplayer.ForEach(displayer => displayer.gameObject.SetActive(false));

        public void ShowRackSpaceDisplayerULevel(int startU, int range) =>
            AvailableUDisplayer.Where(displayer => startU <= displayer.ULevel && displayer.ULevel < startU+range).ToList()
                .ForEach(displayer => displayer.ShowULevel());
        public void HideRackSpaceDisplayerULevel() =>AvailableUDisplayer.ForEach(displayer => displayer.HideULevel());
        

        /// 剩餘U空間的每種大小尺吋 (即時計算)
        public List<int> EachSizeOfEmptyU
            => AvailableULevels
                .OrderBy(u => u)
                .Aggregate(new List<List<int>>(), (resultGroup, uSlot) =>
                {
                    if (resultGroup.Count == 0 || resultGroup.Last().Last() + 1 != uSlot)
                        resultGroup.Add(new List<int> { uSlot });
                    else
                        resultGroup.Last().Add(uSlot);
                    return resultGroup;
                })
                .Select(group => group.Count) // 取得每個區間的 U 數
                .OrderByDescending(u => u) // 讓較大 U 優先匹配
                .ToList();

        /// 判斷設備的條件，是否可放入此機櫃 
        public bool IsDeviceSuitable(DeviceModelDataExtended deviceData)
            => (deviceData.information.watt <= RemainOfWatt //剩餘電力
                && (deviceData.information.weight <= RemainOfWeight) //剩餘負重
                && EachSizeOfEmptyU.Any(size => size >= deviceData.information.heightU)); //可以空間尺吋

        
        #region 機櫃即時計算數據 - 用電量、負重、U空間

        /// 總用電量 (即時計算)
        public float UsageOfWatt => Containers.Sum(device => device.information.watt);
        public float RemainOfWatt => information.watt_limit - UsageOfWatt;
        public float UsagePercentOfWatt => UsageOfWatt / information.watt_limit * 100f;
        public float RemainPercentOfWatt => 100f - UsagePercentOfWatt;

        /// 總負重量 (即時計算)
        public float UsageOfWeight => Containers.Sum(device => device.information.weight);

        public float RemainOfWeight => information.weight_limit - UsageOfWeight;
        public float UsagePercentOfWeight => UsageOfWeight / information.weight_limit * 100f;
        public float RemainPercentOfWeight => 100f - UsagePercentOfWeight;

        /// U空間總使用量 (即時計算)
        public int UsageOfHeightU => Containers.Sum(device => device.information.heightU);

        public int RemainOfHeightU => information.heightU - UsageOfHeightU;
        public float UsagePercentOfHeightU => (float)UsageOfHeightU / information.heightU * 100f;
        public float RemainPercentOfHeightU => 100f - UsagePercentOfHeightU;

        #endregion
    }

    /// Revit模型資料格式 - 設備 (擴增) 
    [Serializable]
    public class DeviceModelDataExtended : DeviceModelData
    {
        /// 設備型號  {Brocade-7X-8-sim}
        public string DeviceType
        {
            get
            {
                string result = DeviceName.Split("+")[0].Trim();
                int lastDashIndex = result.LastIndexOf('-'); //去除HeightU

                if (result.Contains("Brocade") == false)
                {
                    result = result.Substring(0, lastDashIndex);
                }
                return result;
            }
        }

        [HideInInspector]
        public RackModelDataExtended rackModelData;
    }

    /// COBie資料格式 (擴增)
    [Serializable]
    public class CoBieDataExtended : CoBieData
    {
        /// COBie欄位對照表 - 中文
        public static Dictionary<string, string> CobieColumnNameZh => _cobieColumnNameZh ??=
            new Dictionary<string, string>()
            {
                { "component_description", "描述/設備名稱" },
                { "component_assetIdentifier", "FM資產識別字" },
                { "component_serialNumber", "產品序號" },
                { "component_installationDate", "安裝日期" },
                { "component_tagName", "設備編碼" },
                { "component_warrantyDurationPart", "保固時間" },
                { "component_warrantyDurationUnit", "保固時間單位" },
                { "component_warrantyGuarantorLabor", "保固廠商" },
                { "component_warrantyStartDate", "保固開始時間" },
                { "component_warrantyEndDate", "保固結束時間" },
                { "document_inspection", "保養檢查表" },
                { "document_handout", "使用手冊" },
                { "document_drawing", "圖說" },
                { "contact_company", "聯絡單位公司" },
                { "contact_department", "聯絡單位部門" },
                { "contact_email", "聯絡人Email" },
                { "contact_familyName", "聯絡人姓氏" },
                { "contact_givenName", "聯絡人名字" },
                { "contact_phone", "聯絡人電話" },
                { "contact_street", "聯絡人地址" },
                { "facility_name", "專案棟別名稱" },
                { "facility_projectName", "專案名稱" },
                { "facility_siteName", "項目地點" },
                { "equipment_supplier", "供應廠商" },
                { "floor_name", "樓層名稱/所屬樓層" },
                { "space_name", "項目地點" },
                { "space_roomTag", "空間名稱" },
                { "system_category", "系統類別 DCS、DCN" },
                { "system_name", "系統名稱" },
                { "type_category", "OmniClass編碼" },
                { "type_expectedLife", "使用年限" },
                { "type_manufacturer", "製造廠商" },
                { "type_modelNumber", "產品型號" },
                { "type_name", "設備品類名稱" },
                { "type_replacementCost", "設備售價" },
                { "type_accessibilityPerformance", "無障礙功能" },
                { "type_shape", "形狀" },
                { "type_size", "尺寸" },
                { "type_color", "顏色" },
                { "type_finish", "完成面" },
                { "type_grade", "設備分級" },
                { "type_material", "材質" },
            };

        private static Dictionary<string, string> _cobieColumnNameZh;
    }

    #region Data基本格式

    /// Revit模型資料格式 
    public abstract class RevitModelData
    {
        public string devicePath;
        public CoBieDataExtended information;
    }

    public abstract class RevitModelDataExtended : RevitModelData
    {
        /// 模型物件 (手動設定 / 被點擊呼叫時再設定)
        public Transform Model
        {
            get => _model ??= Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).FirstOrDefault(target =>
                RevitHelper.GetDevicePath(target.name).Contains(devicePath, StringComparison.OrdinalIgnoreCase));
            set => _model = value;
        }
        [NonSerialized]
        private Transform _model;

        /// 將DevicePath用"+" Split分開
        protected string[] DevicePathSplit => _devicePathSplit ??= devicePath.Split("+");

        private string[] _devicePathSplit;

        /// 設備樓層
        public string Floor => DevicePathSplit[3].Trim();

        /// 設備分類 {DCR, DCN, DCS}
        public EnumReviteModelSystem System => DevicePathSplit[5].Trim().StringToEnum<EnumReviteModelSystem>();

        /// 設備種類 (用關鍵字來判斷)
        public EnumReviteModelKind DeviceKind
        {
            get
            {
                if (devicePath.Contains("DCR", StringComparison.OrdinalIgnoreCase) ||
                    devicePath.Contains("ATEN", StringComparison.OrdinalIgnoreCase))
                    return EnumReviteModelKind.Rack;
                if (devicePath.Contains("DCS", StringComparison.OrdinalIgnoreCase))
                    return EnumReviteModelKind.Server;
                if (devicePath.Contains("ROUTER", StringComparison.OrdinalIgnoreCase))
                    return EnumReviteModelKind.Router;
                if (devicePath.Contains("SWITCH", StringComparison.OrdinalIgnoreCase))
                    return EnumReviteModelKind.Switch;
                return EnumReviteModelKind.Undefined;
            }
        }

        /// 設備種類ICON，由外部呼叫時進行設置，以避免與其它類別建立耦合
        public Sprite DeviceIcon { get; set; } = null;

        /// 設備名稱 {Brocade-7X-8-sim+流水號}
        public string DeviceName => devicePath.Split(":")[1].Trim();
    }

    /// Revit模型資料格式 - 機櫃 
    public abstract class RackModelData : RevitModelDataExtended
    {
        /// 機櫃裡的所有設備
        public List<DeviceModelDataExtended> Containers = new();
    }

    /// Revit模型資料格式 - 設備類型
    public abstract class DeviceModelData : RevitModelDataExtended
    {
        public string rackDevicePath;
        public int rackLocation;
    }

    /// COBie資料 格式
    public abstract class CoBieData
    {
        public float watt_limit; //總電力負荷量
        public float weight_limit; //總重量負荷量
        public string component_description;
        public string component_assetIdentifier;
        public string component_serialNumber;
        public string component_installationDate;
        public string component_tagName;
        public string component_warrantyDurationPart;
        public string component_warrantyDurationUnit;
        public string component_warrantyGuarantorLabor;
        public string component_warrantyStartDate;
        public string component_warrantyEndDate;
        public string document_inspection;
        public string document_handout;
        public string document_drawing;
        public string contact_company;
        public string contact_department;
        public string contact_email;
        public string contact_familyName;
        public string contact_givenName;
        public string contact_phone;
        public string contact_street;
        public string facility_name;
        public string facility_projectName;
        public string facility_siteName;
        public string equipment_supplier;
        public string floor_name;
        public string space_name;
        public string space_roomTag;
        public string system_category;
        public string system_name;
        public string type_category;
        public string type_expectedLife;
        public string type_manufacturer;
        public string type_modelNumber;
        public string type_name;
        public string type_replacementCost;
        public string type_accessibilityPerformance;
        public string type_shape;
        public string type_size;
        public string type_color;
        public string type_finish;
        public string type_grade;
        public string type_material;
        public float length;
        public float width;
        public float height;
        public int heightU;
        public int watt;
        public int weight;
    }

    #endregion

    // Revit模型System分類列表 (DCR, DCS, DCN)
    public enum EnumReviteModelSystem
    {
        Dcr,
        Dcs,
        Dcn,
    }

    /// Revit模型分類 (RACK, SERVER, ROUTER, SWITCH)
    public enum EnumReviteModelKind
    {
        Undefined,
        Rack,
        Server,
        Router,
        Switch
    }
}