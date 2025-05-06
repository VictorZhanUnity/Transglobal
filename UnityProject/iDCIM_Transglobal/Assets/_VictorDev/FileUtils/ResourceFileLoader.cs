using System;
using System.Collections;
using UnityEngine;
using VictorDev.Common;
using VictorDev.Parser;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.FileUtils
{
    public class ResourceFileLoader : SingletonMonoBehaviour<ResourceFileLoader>
    {
        /// 讀取文字檔 / JSON檔
        /// <para>+ path: Resources資料夾裡的路徑，不用包含Resources與副檔名.json</para>
        /// <para>+ 例如: myJsonData</para>
        public static Coroutine LoadJsonFile(string path, Action<string> onSuccess, Action<string> onFailed=null)
        {
            Debug.Log($"LoadJsonFile... {path}", Instance, EmojiEnum.Monitor);
            void LoadFile()
            {
                // 加載 JSON 文件
                TextAsset jsonFile = Resources.Load<TextAsset>(path.Trim());
                Debug.Log($"LoadJsonFile... Done!\n{JsonHelper.PrintJSONFormatting(jsonFile.text)}", Instance, EmojiEnum.DataBox);
                if (jsonFile != null) onSuccess?.Invoke(jsonFile.text);
                else onFailed?.Invoke("JSON 文件讀取失敗！");
                
            }
            
            if (Application.isEditor)
            {
                LoadFile();
                return null;
            }
            else
            {
                IEnumerator LoadFileCoroutine()
                {
                    LoadFile();
                    yield return null;
                }
                return Instance.StartCoroutine(LoadFileCoroutine()); 
            }
        }
    }
}
