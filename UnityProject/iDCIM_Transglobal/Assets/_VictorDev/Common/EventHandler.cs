using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VictorDev.Common
{
    /// <summary>
    /// 事件處理
    /// </summary>
    public static class EventHandler
    {
        /// <summary>
        /// 目前是否正在使用輸入框
        /// </summary>
        public static bool IsUsingInputField
        {
            get
            {
                GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
                if (selectedObject != null)
                {
                    // 檢查是否為 Unity 的 InputField
                    if (selectedObject.GetComponent<InputField>() != null)
                        return true;
                    // 檢查是否為 TextMeshPro 的 TMP_InputField
                    if (selectedObject.GetComponent<TMP_InputField>() != null)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 目前鼠標是否位於UI組件上
        /// </summary>
        public static bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    }
}
