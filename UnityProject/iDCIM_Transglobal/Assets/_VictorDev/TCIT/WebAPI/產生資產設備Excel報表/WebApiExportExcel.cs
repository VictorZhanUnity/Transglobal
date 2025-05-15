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
    public class WebApiExportExcel : SingletonMonoBehaviour<WebApiExportExcel>
    {
        [Header("[Request設定]")] [SerializeField]
        private WebAPI_Request apiRequest;

        
        public static void UploadData<T>(T classObj, Action<List<Data_Blackbox>> onSuccess,
            Action<long, string> onFailed = null) => UploadData(JsonConvert.SerializeObject(classObj), onSuccess, onFailed);
        
        /// 根據TagName取得即時資訊
        public static void UploadData(string jsonString, Action<List<Data_Blackbox>> onSuccess,
            Action<long, string> onFailed = null)
        {
            Instance.apiRequest.SetRawJsonData(jsonString);
            WebAPI_Caller.SendRequest(Instance.apiRequest, OnSuccessHandler, onFailed);
            return;

            void OnSuccessHandler(long responseCode, string jsonString)
            {
                Debug.Log($"[{responseCode}] OnSuccessHandler\n{JsonHelper.PrintJSONFormatting(jsonString)}");
                onSuccess?.Invoke(JsonConvert.DeserializeObject<List<Data_Blackbox>>(jsonString));
            }
        }

        /// 欲傳送的資料
        [Serializable]
        public class SendData
        {
            public string batching;
            public int index;
            public bool isFinalBatch;
        }
    }
}