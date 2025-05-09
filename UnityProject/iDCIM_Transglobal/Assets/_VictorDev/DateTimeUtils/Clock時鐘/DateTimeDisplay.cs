using System;
using _VictorDEV.DateTimeUtils;
using TMPro;
using UnityEngine;
using VictorDev.Common;
using static VictorDev.DateTimeUtils.Clock;

namespace VictorDev.DateTimeUtils
{
    /// <summary>
    /// ����ɶ���ܾ�
    /// </summary>

    public class DateTimeDisplay : MonoBehaviour, IClockReceiver
    {
        [Header(">>> ����ɶ��r��榡 {MM/dd ddd} =>")]
        [SerializeField] private string format = "MM/dd ddd";

        [Header(">>> �O�_���^�媩")]
        [SerializeField] private bool isEng = true;

        [Header(">>> [�ե�]�A�Y�L���w�h�q�����^��")]
        [SerializeField] private TextMeshProUGUI txt;

        public void OnReceive(DateTime dateNow)
        {
            txt ??= GetComponent<TextMeshProUGUI>();
            txt.SetText(dateNow.ToString(format, DateTimeHelper.GetCulture(isEng)));
        }
    }
}
