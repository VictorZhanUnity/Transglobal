using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using VictorDev.Common;
using VictorDev.Net.WebAPI;
using VictorDev.Parser;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.TCIT.WebAPI
{
    /// [WebAPI] - 取得即時資料
    /// <para>[DataSync文件2.5] 取得即時值</para>
    public class WebApiGetRealtimeData : SingletonMonoBehaviour<WebApiGetRealtimeData>
    {
        [Header("[Request設定]")] [SerializeField]
        private WebAPI_Request requestGetRealtimeData;

        /// 根據TagName取得即時資訊
        public static void GetRealtimeData(SendDataTags sendData, Action<List<Data_Blackbox>> onSuccess,
            Action<long, string> onFailed = null)
        {
            WebApiLoginManager.CheckToken(Instance.requestGetRealtimeData);
            Instance.requestGetRealtimeData.SetRawJsonData(JsonConvert.SerializeObject(sendData));
            WebAPI_Caller.SendRequest(Instance.requestGetRealtimeData, OnSuccessHandler, onFailed);
            return;

            void OnSuccessHandler(long responseCode, string jsonString)
            {
                Debug.Log($"[{responseCode}] OnSuccessHandler\n{JsonHelper.PrintJSONFormatting(jsonString)}");
                onSuccess?.Invoke(JsonConvert.DeserializeObject<List<Data_Blackbox>>(jsonString));
            }
        }

        /// 欲傳送的Tags列表
        [Serializable]
        public class SendDataTags
        {
            public List<string> tags;

            /// 建構子
            public SendDataTags()
            {
            }

            public SendDataTags(List<string> tags) => this.tags = tags;
        }
    }
}