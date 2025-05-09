using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace _VictorDEV.DateTimeUtils
{
    /// 自動調整Options內容至目前的年份
    [RequireComponent(typeof(TMP_Dropdown))]
    public class AutoAdjustYearDetector : MonoBehaviour
    {
        [Header(">>> 增減年數")]
        [SerializeField] private int adjustYear = 0;

        private void Start()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            string firstLabel = DropdownInstance.options[0].text.Trim();
            bool isInclueLabel = firstLabel.Contains("年");
            
            int firstOptionYear = int.Parse(isInclueLabel? firstLabel.Replace("年", ""): firstLabel);
            int currentYear = DateTime.Now.Year;
            // 動態增加未來的年份
            for (int year = firstOptionYear + 1; year <= currentYear+adjustYear; year++)
            {
                if (DropdownInstance.options.Exists(option => option.text == year.ToString()) == false)
                {
                    DropdownInstance.options.Add(new TMP_Dropdown.OptionData(year.ToString() + (isInclueLabel? "年" :"")));
                }
            }
            
            //遞減排序
            DropdownInstance.options = DropdownInstance.options.OrderByDescending(optionData => int.Parse(isInclueLabel? optionData.text.Trim().Replace("年", ""): optionData.text.Trim())).ToList();
            DropdownInstance.value = 0;
        }

        private void OnDisable() => DropdownInstance.value = 0;
        
        private TMP_Dropdown DropdownInstance => _dropdown ??= GetComponent<TMP_Dropdown>();
        private TMP_Dropdown _dropdown;
       
    }
}