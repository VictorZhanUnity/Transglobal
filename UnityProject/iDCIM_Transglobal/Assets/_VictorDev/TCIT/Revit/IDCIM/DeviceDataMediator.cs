using System.Collections.Generic;
using System.Linq;
using _VictorDEV.Revit;

namespace VictorDev.RevitUtils.IDCIM
{
    public static class DeviceDataMediator
    {
        /// 篩選出{Server、Router、Switch}類的設備資料項
        public static List<DeviceModelDataExtended> GetDeviceDataOfModelKind(EnumReviteModelKind modelKind,
            List<RackModelDataExtended> rackDataList)
            => rackDataList.SelectMany(rack => rack.Containers).Where(device => device.DeviceKind.Equals(modelKind))
                .ToList();
        
        /// 計算所有機櫃加總的電容量上限
        public static float GetWattLimitOfAllRack(List<RackModelDataExtended> rackDataList) =>rackDataList.Sum(rack => rack.information.watt_limit);
        /// 計算所有機櫃加總的承重上限
        public static float GetWeightLimitOfAllRack(List<RackModelDataExtended> rackDataList) =>rackDataList.Sum(rack => rack.information.weight_limit);
        /// 計算所有機櫃加總的U層數上限
        public static float GetHeightLimitOfAllRack(List<RackModelDataExtended> rackDataList) =>rackDataList.Sum(rack => rack.information.heightU);
        
        /// 計算所有機櫃加總的電容量上限
        public static float GetWattUsageOfAllRack(List<RackModelDataExtended> rackDataList) =>rackDataList.Sum(rack => rack.UsageOfWatt);
        /// 計算所有機櫃加總的承重上限
        public static float GetWeightUsageOfAllRack(List<RackModelDataExtended> rackDataList) =>rackDataList.Sum(rack => rack.UsageOfWeight);
        /// 計算所有機櫃加總的U層數上限
        public static float GetHeightUsageOfAllRack(List<RackModelDataExtended> rackDataList) =>rackDataList.Sum(rack => rack.UsageOfHeightU);
    }
}