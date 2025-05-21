using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using VictorDev.Common;
using VictorDev.FileUtils;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.Net.WebAPI.TCIT
{
    /// 後端第二代 - 動態生成機房設備Excel報表
    public class WebApiExcelGenerator : SingletonMonoBehaviour<WebApiExcelGenerator>
    {
        /// 1. 將JSON資料進行分段批次，傳給Prepare做緩存
        public static void ExcelPrepare<T>(T data, Action<long, byte[]> onSuccess = null,
            Action<long, string> onFailed = null, int? chunkSize = null)
            => ExcelPrepare(JsonConvert.SerializeObject(data), onSuccess, onFailed, chunkSize);

        private Action<long, byte[]> onSuccess;
        private Action<long, string> onFailed;

        /// 將JSON字串進行分段批次，傳給Prepare做緩存
        public static void ExcelPrepare(string deviceJsonString, Action<long, byte[]> onSuccess = null,
            Action<long, string> onFailed = null, int? chunkSize = null)
        {
            Instance.onSuccess = onSuccess;
            Instance.onFailed = onFailed;
            Instance.sendString = deviceJsonString;
            Instance.jsonChunkSize = chunkSize ?? Instance.jsonChunkSize;
            Instance.jsonChunkList = StringHelper.SplitString(deviceJsonString, Instance.jsonChunkSize);
            Instance.jsonChunkCounter = 0;
            Debug.Log($"JonChunkList Count: {Instance.jsonChunkList.Count} / Each ChunkSize: {Instance.jsonChunkSize}",
                Instance, EmojiEnum.DataBox);
            Instance.SendExcelPrepareDataRecursive();
        }

        /// 遞回呼叫Prepare
        private void SendExcelPrepareDataRecursive()
        {
            PrepareBodyRawData prepareBodyRawData = new PrepareBodyRawData()
            {
                index = jsonChunkCounter,
                isFinalBatch = (jsonChunkCounter == jsonChunkList.Count - 1),
                batchString = jsonChunkList[jsonChunkCounter],
            };
            Debug.Log(prepareBodyRawData, this, EmojiEnum.Robot);
            string rawJsonString = JsonConvert.SerializeObject(prepareBodyRawData);
            requestPrepare.SetRawJsonData(rawJsonString);
            WebAPI_Caller.CallWebAPI(requestPrepare, OnPrepareSuccessHandler);

            void OnPrepareSuccessHandler(long responseCode, Dictionary<string, string> arg2)
            {
                Debug.Log($"JsonChunk[{jsonChunkCounter}] - onPrepareSuccessHandler[{responseCode}]", this,
                    EmojiEnum.Done);
                if (++jsonChunkCounter < jsonChunkList.Count) SendExcelPrepareDataRecursive();
                else ExcelStart();
            }
        }

        [Button]
        private void ExcelPrepareSendString() => ExcelPrepare(sendString);

        /// 2. 開始生成Excel報表
        [Button]
        private void ExcelStart()
        {
            Debug.Log("ExcelStart", this, EmojiEnum.Robot);
            WebAPI_Caller.CallWebAPI(requestStart, OnStartSuccessHandler);

            void OnStartSuccessHandler(long responseCode, Dictionary<string, string> arg2)
            {
                Debug.Log($"OnStartSuccessHandler[{responseCode}]", this, EmojiEnum.Done);
                ExcelExport();
            }
        }

        /// 3. 下載Excel報表
        [Button]
        public void ExcelExport()
        {
            Debug.Log("ExcelExport", this, EmojiEnum.Robot);
            WebAPI_Caller.CallWebAPI(requestExport, OnExportSuccessHandler, onFailed);

            void OnExportSuccessHandler(long responseCode, byte[] excelData)
            {
                Debug.Log($"OnExportSuccessHandler[{responseCode}]", this, EmojiEnum.Done);
                Debug.Log($"excelData length: {excelData.Length}", this, EmojiEnum.Done);
                onSuccess ??= OnSuccessHandlerDefault;
                onSuccess?.Invoke(responseCode, excelData);
            }

            void OnSuccessHandlerDefault(long responseCode, byte[] excelData)
            {
                string[] filePath = FileHelper.DownloadHandlerFile(excelData, "DownloadExcel.xlsx");
                //將資料夾路徑COPY至剪貼簿
                GUIUtility.systemCopyBuffer = filePath[0];
            }
        }

        #region Variables

        [Header("[設定] - JSON分割大小")] [SerializeField]
        private int jsonChunkSize = 500000;

        [SerializeField] 
        private List<string> jsonChunkList;
        private int jsonChunkCounter = 0;
        
        [Foldout("[WebAPI Request]")] [SerializeField]
        private WebAPI_Request requestPrepare;

        [Foldout("[WebAPI Request]")] [SerializeField]
        private WebAPI_Request requestStart;

        [Foldout("[WebAPI Request]")] [SerializeField]
        private WebAPI_Request requestExport;

        [Foldout("測試")] [TextArea(1, 30)] [SerializeField]
        private string sendString;

        #endregion

        /// 呼叫Prepare的Body Raw格式
        [SerializeField]
        public class PrepareBodyRawData
        {
            /// 第幾批(從0起算)
            public int index;
            /// 最後一批要為true
            public bool isFinalBatch;
            /// 批次JSON字串
            public object batchString;

            public override string ToString()
                => $"SendJsonChunkToPrepare[{index}]:\n{batchString}\nisFinalBatch: {isFinalBatch}";
        }
    }
}