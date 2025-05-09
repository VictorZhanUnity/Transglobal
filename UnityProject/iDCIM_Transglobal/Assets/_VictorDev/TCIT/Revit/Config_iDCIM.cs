using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VictorDev.Common;

namespace _VictorDEV.Revit
{
    public class Config_iDCIM: SingletonMonoBehaviour<Config_iDCIM>
    {
        /// 取得告警類別
        public static AlarmSystemSetting GetAlarmSystemSetting(string tagName)
            => Instance.alarmSystemSettings.FirstOrDefault(setting =>
                setting.keywords.Any(keyword => tagName.Contains(keyword)));

        [Header(">>> 告警資料設定對照表")]
        public List<AlarmSystemSetting> alarmSystemSettings = new List<AlarmSystemSetting>()
        {
            new AlarmSystemSetting(){ label = "電壓", keywords = new List<string>() { "Utility", "UPS", "PDU" }},
            new AlarmSystemSetting(){ label = "溫度", keywords = new List<string>() { "RT"}},
            new AlarmSystemSetting(){ label = "濕度", keywords = new List<string>() { "RH"}},
            new AlarmSystemSetting(){ label = "煙霧", keywords = new List<string>() { "Smoke"}},
            new AlarmSystemSetting(){ label = "消防", keywords = new List<string>() { "Fire"}},
            new AlarmSystemSetting(){ label = "空調", keywords = new List<string>() { "AirConditioner"}, keywordExcludes = new List<string>() { "Leak"}},
            new AlarmSystemSetting(){ label = "漏水", keywords = new List<string>() { "Leak"}},
        };
        [Serializable]
        public struct AlarmSystemSetting
        {
            ///中文字
            public string label;    
            /// 圖示
            public Sprite icon;
            ///關鍵字
            public List<string> keywords;   
            //排除關鍵字
            public List<string> keywordExcludes;   
        }
    }
}