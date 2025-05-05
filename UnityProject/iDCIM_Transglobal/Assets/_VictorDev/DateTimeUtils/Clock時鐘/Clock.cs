using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VictorDev.Common;

namespace VictorDev.DateTimeUtils
{
    public class Clock : MonoBehaviour
    {
        [Header(">>>接收器{現在時間}")]
        [SerializeField] private List<MonoBehaviour> _iClockReceiver;
        private List<IClockReceiver> iClockReceiver { get; set; }

        [Header(">>> 是否為24小時制")]
        [SerializeField] private bool is24Hrs = true;

        [Header(">>> 是否顯示秒數")]
        [SerializeField] private bool isShowSec = false;

        [Header(">>> AM/PM語言")]
        [SerializeField] private enumLang lang = enumLang.英文;

        [Header(">>> Invoke 年/月/日")]
        public UnityEvent<string> onSetDateEvent = new UnityEvent<string>();
        [Header(">>> Invoke 星期幾")]
        public UnityEvent<string> onSetDayEvent = new UnityEvent<string>();

        [Space(10)]
        [SerializeField] private TextMeshProUGUI txtTime;

        private CultureInfo cultureInfo { get; set; }
        private bool jumper = true;

        private string langFormat
        {
            get
            {
                switch (lang)
                {
                    case enumLang.中文: return "zh-CN";
                    case enumLang.英文: return "en-US";
                }
                return "";
            }
        }

        private void Awake() => iClockReceiver = _iClockReceiver.OfType<IClockReceiver>().ToList();

        private void Start()
        {
            IEnumerator enumerator()
            {
                while (true)
                {
                    UpdateClock();
                    yield return new WaitForSeconds(1f); // 每秒更新一次
                }
            }
            cultureInfo = new CultureInfo(langFormat);
            StartCoroutine(enumerator());
        }
        private void UpdateClock()
        {
            string symbol = jumper ? ":" : " ";
            string format = (is24Hrs) ? $"HH{symbol}mm" : $"tt hh{symbol}mm";
            if (isShowSec) format += $"{symbol}ss";

            DateTime dateNow = DateTime.Now;

            float timeAdjust = 0f;
            dateNow = dateNow.AddHours(timeAdjust); //往回推 for Demo

            string time = dateNow.ToString(format, cultureInfo);
            string date = dateNow.ToString("yyyy/MM/dd", cultureInfo);
            string day = dateNow.ToString("ddd", cultureInfo);

            txtTime.SetText(time);
            onSetDateEvent?.Invoke(date);
            onSetDayEvent?.Invoke(day);

            iClockReceiver?.ForEach(receiver => receiver?.OnReceive(dateNow));
            jumper = !jumper;
        }

        [ContextMenu("- 更新Receiver")]
        private void OnValidate()
        {
            txtTime ??= GetComponent<TextMeshProUGUI>();
            if (txtTime != null)
            {
                cultureInfo = new CultureInfo(langFormat);
                UpdateClock();
            }

            _iClockReceiver = ObjectHelper.CheckTypoOfList<IClockReceiver>(_iClockReceiver);
        }

        private enum enumLang { 中文, 英文 }

        public interface IClockReceiver
        {
            void OnReceive(DateTime dateNow);
        }
    }
}
