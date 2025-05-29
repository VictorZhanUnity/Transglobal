using System;
using System.Diagnostics;
using System.IO;
using _VictorDEV.DateTimeUtils;
using JetBrains.Annotations;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.FileUtils
{
    public static class FileHelper
    {
        /// 依BLOB資料生成檔案
        /// <para>return string[]: [資料夾路徑][檔案名稱(包含副檔名)]</para>
        public static string[] DownloadHandlerFile(byte[] fileData, [CanBeNull] string fileFullName = null, bool isAutoOpen = true)
        {
            string folderURL = AppDomain.CurrentDomain.BaseDirectory;
            fileFullName ??= $"DownloadFile-{DateTime.Today.ToString(DateTimeHelper.FullDateFormat)}";
            string filePath = Path.Combine(folderURL, fileFullName);
            File.WriteAllBytes(filePath, fileData);
            Debug.Log($"檔案已儲存至:{filePath}", typeof(FileHelper), EmojiEnum.Download);

            if (isAutoOpen)
            {
                try
                {
                    // 開啟 Excel 檔案
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true // 讓作業系統選擇合適的應用程式來開啟
                    });
                }
                catch (Exception e)
                {
                    Debug.LogError($"開啟{fileFullName}時發生錯誤: " + e.Message);
                }
            }
            return new string[] { folderURL, fileFullName };
        }
    }
}