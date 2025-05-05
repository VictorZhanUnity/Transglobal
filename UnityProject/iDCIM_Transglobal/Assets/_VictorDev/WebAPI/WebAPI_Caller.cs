using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using VictorDev.Async.CoroutineUtils;
using VictorDev.Parser;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.Net.WebAPI
{
    /// <summary>
    /// WebAPI呼叫 (以Coroutine方式呼叫)
    /// </summary>
    public abstract class WebAPI_Caller
    {
        /// <summary>
        /// 呼叫WebAPI(用URL，預設RequestPacakge) (單一JSON資料)
        /// <para>+ 泛型 [T] request種類: string / WebAPI_Request</para>
        /// <para>+ onSuccess: 回傳原始資料string</para>
        /// </summary>
        public static void SendRequest<T>(T requestInput, Action<long, string> onSuccess, Action<long, string> onFailed = null) where T : class
        {
            //處理泛型進行轉換
            WebAPI_Request request = null;
            if (requestInput is string url) request = new WebAPI_Request(url);
            else if (requestInput is WebAPI_Request req) request = req;

            onFailed ??= (responseCode, msg) =>
            {
                Debug.Log($"[{responseCode}] onFailed: {msg} / {request.name}");
            };

            if (request != null) CoroutineHandler.RunCoroutine_Old(SendWebRequestCoroutine(request, onSuccess, onFailed));
            else Debug.LogWarning($"Type of WebAPI Request is Error!");
        }
        /// <summary>
        /// 發送WebRequest
        /// </summary>
        private static IEnumerator SendWebRequestCoroutine(WebAPI_Request request, Action<long, string> onSuccess, Action<long, string> onFailed)
        {
            void RequestErrorChecker(UnityWebRequest webRequest)
            {
                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogWarning("Error: " + webRequest.error);
                }
            }

            switch (request.requestMethod)
            {
                case enumRequestMethod.GET:
                    using (UnityWebRequest webRequest = UnityWebRequest.Get(request.url))
                    {
                        RequestSettingHandler(request, webRequest);
                        // 發送請求
                        yield return webRequest.SendWebRequest();

                        RequestErrorChecker(webRequest);
                        // 處理結果資訊
                        RequestResponseHandler(webRequest, onSuccess, onFailed);
                    }
                    break;
                case enumRequestMethod.POST:
                    switch (request.bodyType)
                    {
                        case enumBody.none:
                            using (UnityWebRequest webRequest = new UnityWebRequest(request.url, "POST"))
                            {
                                RequestSettingHandler(request, webRequest);
                                // 發送請求
                                yield return webRequest.SendWebRequest();
                                RequestErrorChecker(webRequest);
                                // 處理結果資訊
                                RequestResponseHandler(webRequest, onSuccess, onFailed);
                            }
                            break;
                        case enumBody.formData:
                            using (UnityWebRequest webRequest = UnityWebRequest.Post(request.url, request.formData))
                            {
                                RequestSettingHandler(request, webRequest);
                                // 發送請求
                                yield return webRequest.SendWebRequest();
                                RequestErrorChecker(webRequest);
                                // 處理結果資訊
                                RequestResponseHandler(webRequest, onSuccess, onFailed);
                                break;
                            }
                        case enumBody.rawJSON:
                            using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(request.url, request.BodyJSON))
                            {
                                RequestSettingHandler(request, webRequest);
                                // 發送請求
                                yield return webRequest.SendWebRequest();
                                RequestErrorChecker(webRequest);
                                // 處理結果資訊
                                RequestResponseHandler(webRequest, onSuccess, onFailed);
                                break;
                            }
                    }
                    break;
            }

        }
        /// <summary>
        /// 設定Request各項參數
        /// </summary>
        private static void RequestSettingHandler(WebAPI_Request request, UnityWebRequest webRequest)
        {
            // 設定回應資料型態參數(Header)
            webRequest.SetRequestHeader("Content-Type", request.contentType);
            //Debug.Log($"\t[Content-Type] {request.contentType}");

            //設定Authorization參數
            if (request.authorizationType != enumAuthorization.NoAuth)
            {
                webRequest.SetRequestHeader("Authorization", $"{request.authorizationType} {request.token}");
                //  Debug.Log($"\t[Authorization: {request.authorizationType}] Token: {request.token}");
            }

            DownloadHandler downloadHandler;
            if (request.requestMethod == enumRequestMethod.POST || request.requestMethod == enumRequestMethod.PUT)
            {
                switch (request.bodyType)
                {
                    case enumBody.formData: //不用任何動作
                        break;
                    case enumBody.rawJSON:
                        if (string.IsNullOrEmpty(request.BodyJSON) == false)
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(request.BodyJSON);
                            webRequest.uploadHandler = new UploadHandlerRaw(bytes);
                        }
                        break;
                }
            }
            downloadHandler = new DownloadHandlerBuffer();
            webRequest.downloadHandler = downloadHandler;
        }

        /// <summary>
        /// WebAPI回應Request後的處理
        /// </summary>
        private static void RequestResponseHandler(UnityWebRequest webRequest, Action<long, string> onSuccess, Action<long, string> onFailed)
        {
            if (webRequest.result == UnityWebRequest.Result.ConnectionError
               || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                //失敗
                onFailed?.Invoke(webRequest.responseCode, webRequest.error);
            }
            else
            {
                //將原始資料Invoke，不進行JSON解析
                onSuccess?.Invoke(webRequest.responseCode, webRequest.downloadHandler.text);
            }
        }


        /// <summary>
        /// 呼叫WebAPI(用URL，預設RequestPacakge) (單一JSON資料)
        /// </summary>
        /// <param name="url">網址</param>
        /// <param name="onSuccess">成功，回傳Dictionary<欄位名，值></param>
        /// <param name="onFailed">失敗，回傳錯誤訊息</param>
        public static IEnumerator CallWebAPI(string url, Action<long, Dictionary<string, string>> onSuccess, Action<long, string> onFailed = null)
             => CallWebAPI(new WebAPI_Request(url), onSuccess, onFailed);

        /// <summary>
        /// 呼叫WebAPI(用RequestPackage) (單一JSON資料)
        /// </summary>
        /// <param name="url">網址</param>
        /// <param name="onSuccess">成功，回傳Dictionary<欄位名，值></param>
        /// <param name="onFailed">失敗，回傳錯誤訊息</param>
        public static IEnumerator CallWebAPI(WebAPI_Request requestPackage, Action<long, Dictionary<string, string>> onSuccess, Action<long, string> onFailed = null)
             => CoroutineHandler.RunCoroutine_Old(SendRequestCoroutine(requestPackage, onSuccess, onFailed));

        /// <summary>
        /// 呼叫WebAPI(用URL，預設RequestPacakge) (陣列JSON資料)
        /// </summary>
        /// <param name="url">網址</param>
        /// <param name="onSuccess">成功，回傳Dictionary<欄位名，值></param>
        /// <param name="onFailed">失敗，回傳錯誤訊息</param>
        public static IEnumerator CallWebAPI(string url, Action<long, List<Dictionary<string, string>>> onSuccess, Action<long, string> onFailed = null)
             => CallWebAPI(new WebAPI_Request(url), onSuccess, onFailed);

        /// <summary>
        /// 呼叫WebAPI(用RequestPackage) (陣列JSON資料)
        /// </summary>
        /// <param name="url">網址</param>
        /// <param name="onSuccess">成功，回傳Dictionary<欄位名，值></param>
        /// <param name="onFailed">失敗，回傳錯誤訊息</param>
        public static IEnumerator CallWebAPI(WebAPI_Request requestPackage, Action<long, List<Dictionary<string, string>>> onSuccess, Action<long, string> onFailed = null)
             => CoroutineHandler.RunCoroutine_Old(SendRequestCoroutine(requestPackage, onSuccess, onFailed));

        /// <summary>
        /// ★ 發送請求 (回傳：單一JSON值)
        /// </summary>
        private static IEnumerator SendRequestCoroutine(WebAPI_Request requestPackage, Action<long, Dictionary<string, string>> onSuccess, Action<long, string> onFailed)
        {
            switch (requestPackage.requestMethod)
            {
                case enumRequestMethod.GET:
                    using (UnityWebRequest request = UnityWebRequest.Get(requestPackage.url))
                    {
                        // 設定Request相關資訊
                        DownloadHandler downloadHandler = RequestSetting(requestPackage, request);
                        // 發送請求
                        yield return request.SendWebRequest();
                        // 處理結果資訊
                        ResultHandler(onSuccess, onFailed, request, downloadHandler);
                    }
                    break;
                case enumRequestMethod.POST:
                    using (UnityWebRequest request = new UnityWebRequest(requestPackage.url, "POST"))

                    /*   WWWForm form = new WWWForm();
                       form.AddField("BuildingCode", "TPE");
                       form.AddField("DeviceCode", "HWACOM/TPE/IDC/FL1/DCR/Schneider-ER8222搭電源/Rack1");*/

                    //   using (UnityWebRequest request = UnityWebRequest.Post(requestPackage.url, requestPackage.formData))
                    //using (UnityWebRequest request = UnityWebRequest.Post(requestPackage.url, form))
                    {
                        // 設定Request相關資訊
                        DownloadHandler downloadHandler = RequestSetting(requestPackage, request);
                        // 發送請求
                        yield return request.SendWebRequest();
                        // 處理結果資訊
                        ResultHandler(onSuccess, onFailed, request, downloadHandler);
                    }
                    break;
            }
        }
        /// <summary>
        /// ★ 發送請求 (回傳：JSON值陣列)
        /// </summary>
        private static IEnumerator SendRequestCoroutine(WebAPI_Request requestPackage, Action<long, List<Dictionary<string, string>>> onSuccess, Action<long, string> onFailed)
        {
            switch (requestPackage.requestMethod)
            {
                case enumRequestMethod.GET:
                    using (UnityWebRequest request = UnityWebRequest.Get(requestPackage.url))
                    {
                        // 設定Request相關資訊
                        DownloadHandler downloadHandler = RequestSetting(requestPackage, request);
                        // 發送請求
                        yield return request.SendWebRequest();
                        // 處理結果資訊
                        ResultHandler(onSuccess, onFailed, request, downloadHandler);
                    }
                    break;
                case enumRequestMethod.POST:
                    using (UnityWebRequest request = new UnityWebRequest(requestPackage.url, "POST"))
                    {
                        // 設定Request相關資訊
                        DownloadHandler downloadHandler = RequestSetting(requestPackage, request);
                        // 發送請求
                        yield return request.SendWebRequest();
                        // 處理結果資訊
                        ResultHandler(onSuccess, onFailed, request, downloadHandler);
                    }
                    break;
            }
        }

        /// <summary>
        /// 設定Request相關資訊
        /// </summary>
        private static DownloadHandler RequestSetting(WebAPI_Request requestPackage, UnityWebRequest request)
        {
            // 設定Header資訊
            request.SetRequestHeader("Content-Type", "application/json");
            if (requestPackage.authorizationType != enumAuthorization.NoAuth)
            {
                request.SetRequestHeader("Authorization", $"{requestPackage.authorizationType} {requestPackage.token}");
            }

            DownloadHandler downloadHandler;
            if (requestPackage.requestMethod == enumRequestMethod.POST || requestPackage.requestMethod == enumRequestMethod.PUT)
            {

                if (string.IsNullOrEmpty(requestPackage.BodyJSON) == false)
                {
                    UploadHandler uploadHandler;
                    byte[] bytes = Encoding.UTF8.GetBytes(requestPackage.BodyJSON);
                    uploadHandler = new UploadHandlerRaw(bytes)
                    {
                        contentType = "application/json"
                    };

                    request.uploadHandler = uploadHandler;
                }
            }
            downloadHandler = new DownloadHandlerBuffer();
            request.downloadHandler = downloadHandler;
            return downloadHandler;
        }
        /// <summary>
        /// 處理結果資訊 (回傳：單一JSON值)
        /// </summary>
        private static void ResultHandler(Action<long, Dictionary<string, string>> onSuccess, Action<long, string> onFailed, UnityWebRequest request, DownloadHandler downloadHandler)
        {
            if (request.result == UnityWebRequest.Result.ConnectionError
               || request.result == UnityWebRequest.Result.ProtocolError)
            {
                //失敗
                onFailed?.Invoke(request.responseCode, request.error);
            }
            else
            {
                //成功，回傳Dictionary<欄位名, 值>
                onSuccess?.Invoke(request.responseCode, string.IsNullOrEmpty(downloadHandler.text)? null: JsonHelper.ParseJson(downloadHandler.text));
            }
        }
        /// <summary>
        /// 處理結果資訊 (回傳：JSON值陣列)
        /// </summary>
        private static void ResultHandler(Action<long, List<Dictionary<string, string>>> onSuccess, Action<long, string> onFailed, UnityWebRequest request, DownloadHandler downloadHandler)
        {
            if (request.result == UnityWebRequest.Result.ConnectionError
               || request.result == UnityWebRequest.Result.ProtocolError)
            {
                //失敗
                onFailed?.Invoke(request.responseCode, request.error);
            }
            else
            {
                //成功，回傳Dictionary<欄位名, 值>
                onSuccess?.Invoke(request.responseCode, JsonHelper.ParseJsonArray(downloadHandler.text));
            }
        }


        public static void WebAPI_OnSuccess(long responseCode, string jsonString)
        {
            Debug.Log($">>> WebAPI_OnSuccess - responseCode: {responseCode}");
            JsonHelper.PrintJSONFormatting(jsonString);
        }
        public static void WebAPI_OnFailed(long responseCode, string msg)
        {
            Debug.LogWarning($">>>WebAPI_OnFailed - responseCode: {responseCode}");
            Debug.LogWarning($"\tmsg: {msg}");
        }
    }
}
