using System;
using TMPro;
using UnityEngine;

namespace _VictorDEV.DateTimeUtils
{
    /// 依目前年份與日期，調整Options內容
    [RequireComponent(typeof(TMP_Dropdown))]
    public class AutoAdjustMonthSelector : MonoBehaviour
    {
        public TMP_Dropdown dropdownYear;
        private TMP_Dropdown Dropdown => _dropdown ??= GetComponent<TMP_Dropdown>();
        private TMP_Dropdown _dropdown;

        
        private void Start()
        {
            SelectYearHandler(DateTime.Now.Year);
        }
        
        private void OnEnable() => dropdownYear.onValueChanged.AddListener(OnYearChangedHandler);
        private void OnDisable()
        {
            dropdownYear.onValueChanged.RemoveListener(OnYearChangedHandler);
            Dropdown.value = 0;
        }

        private void OnYearChangedHandler(int index)
        {
            // 假設這裡有一個方法可以獲取選擇的年份
            if (int.TryParse(dropdownYear.options[index].text.Replace("年", ""), out int selectedYear))
            {
                SelectYearHandler(selectedYear);
            }
        }

        private void SelectYearHandler(int selectedYear)
        {
            int currentYear = DateTime.Now.Year;
            Dropdown.options.Clear(); // 清除現有選項
            // 若為今年，則選項數量為一月到目前的月份
            // 若為今年之前，則選項數量為一月到十二月
            int endValue = selectedYear == currentYear ? DateTime.Now.Month : 12;
            for (int i = 0; i < endValue; i++)
            {
                string label = DateTimeHelper.MonthName_ZH[i];
                Dropdown.options.Add(new TMP_Dropdown.OptionData(label));
            }

            Dropdown.value = Dropdown.options.Count-1;
            Dropdown.RefreshShownValue(); // 更新顯示的值
        }
    }
}