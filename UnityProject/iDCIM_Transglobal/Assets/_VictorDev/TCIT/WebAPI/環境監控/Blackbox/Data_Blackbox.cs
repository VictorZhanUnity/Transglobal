using Newtonsoft.Json;
using System;
using _VictorDEV.DateTimeUtils;
using UnityEngine;

[Serializable]
public class Data_Blackbox 
{
    public string tagName;
    public float? value { get; set; }
    public Alarm alarm;

    public override string ToString()
    {
        return $"{tagName}: {value} / alarm: {alarm}";
    }

    [Serializable] //序列化，所以不會是null
    public class Alarm
    {
        public bool IsAlarm => string.IsNullOrEmpty(alarmTime) == false;
        
        /// 發生時間
        public string alarmTime;
        public DateTime AlarmTime => DateTimeHelper.StrToLocalTime(alarmTime);
        /// 描述
        public string alarmMessage;
        /// 是否恢復正常
        public bool isBackToNormal = false;

        public string tagName;
        public int compareOrder;
        public string alarmColor;
    }
}
