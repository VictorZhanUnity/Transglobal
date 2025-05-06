using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using VictorDev.Common;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.FileUtils
{ 
    /// JSON檔讀取處理
    public class SteamAssetsFileLoader : SingletonMonoBehaviour<SteamAssetsFileLoader>
    {
        private string jsonPath;

        public static Coroutine LoadJsonFile(string path, Action<string> onSuccess, Action onFailed = null)
         => Instance.StartCoroutine(Instance.LoadJsonAsync(path, onSuccess, onFailed));

        IEnumerator LoadJsonAsync(string path, Action<string> onSuccess, Action onFailed = null)
        {
            jsonPath = Path.Combine(Application.streamingAssetsPath, path).Replace("\\", "/");;
            Debug.Log($"[SteamAssetsFileLoader] :> {jsonPath}");

            string jsonString = "";

            if (Application.platform == RuntimePlatform.Android)
            {
                // Android 需要用 UnityWebRequest 讀取 StreamingAssets 內的檔案
                using (UnityWebRequest request = UnityWebRequest.Get(jsonPath))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        jsonString = request.downloadHandler.text;
                    }
                    else
                    {
                        onFailed?.Invoke();
                        Debug.LogError($"讀取 JSON 失敗: {request.error}");
                        yield break;
                    }
                }
            }
            else 
            {
                // 其他平台 (Windows, Mac, iOS) 用 StreamReader 逐步讀取 JSON
                if (File.Exists(jsonPath)) 
                {
                    StringBuilder sb = new StringBuilder();
                    using (StreamReader reader = new StreamReader(jsonPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            sb.Append(line);
                            yield return null; // 讓 Unity 在每次讀取一行後，回到主線程，避免卡頓
                        }
                    }

                    jsonString = sb.ToString();
                }
                else
                {
                    onFailed?.Invoke();
                    Debug.LogError("JSON 檔案不存在: " + jsonPath);
                    yield break;
                }
            }

            Debug.Log("JSON 讀取完成");
            onSuccess?.Invoke(jsonString);
        }
    }
}