using System;
using UnityEngine;

namespace VictorDev.Common
{
    /// Log訊息處理 (僅在Editor環境下Log)
    public static class Debug
    {
        /// 在Runtime是否要Log訊息
        public static bool IsLogInRuntime = false;

        public static void Log(object message, object callerClass = null, EmojiEnum emojiEnum = EmojiEnum.None, bool isPrintArrow = true) =>
            LogMessage(EnumLogType.Log, message, callerClass, emojiEnum, isPrintArrow);

        public static void LogWarning(object message, object callerClass = null, EmojiEnum emojiEnum = EmojiEnum.None, bool isPrintArrow = true) =>
            LogMessage(EnumLogType.LogWarning, message, callerClass, emojiEnum, isPrintArrow);

        public static void LogError(object message, object callerClass = null, EmojiEnum emojiEnum = EmojiEnum.None, bool isPrintArrow = true) =>
            LogMessage(EnumLogType.LogError, message, callerClass, emojiEnum, isPrintArrow);

        #region Log訊息處理
        /// Log訊息處理
        private static void LogMessage(EnumLogType logType, object message, object callerClass, EmojiEnum emojiEnum, bool isPrintArrow)
        {
            string msg = $"{EmojiHelper.GetEmoji(emojiEnum)} ";
            msg += callerClass != null ? $"[ {callerClass?.GetType().Name} ] " : "";
            msg += (isPrintArrow? ":> " : " ") + message;
            Action action = null;
            string colorCode;
            switch (logType)
            {
                case EnumLogType.Log:
                    colorCode = "#b7ffbf";
                    msg = $"<color='{colorCode}'>{msg}</color>";
                    action = () => UnityEngine.Debug.Log(msg);
                    break;
                case EnumLogType.LogWarning:
                    colorCode = "#ffad00";
                    msg = $"<color='{colorCode}'>{msg}</color>";
                    action = () => UnityEngine.Debug.LogWarning(msg);
                    break;
                case EnumLogType.LogError:
                    colorCode = "#ff9c9c";
                    msg = $"<color='{colorCode}'>{msg}</color>";
                    action = () => UnityEngine.Debug.LogError(msg);
                    break;
            }

           if(action != null) CheckIsEditorEnviorment(action);
        }
        /// 檢查是否為Editor環境，是才會Log訊息
        private static void CheckIsEditorEnviorment(Action action)
        {
            if (Application.isEditor || IsLogInRuntime) action?.Invoke();
        }
        #endregion

        private enum EnumLogType
        {
            Log,
            LogWarning,
            LogError
        }
    }
}