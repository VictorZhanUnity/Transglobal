using System;
using System.Collections.Generic;
using System.Linq;

namespace VictorDev.Common
{
    public static class EnumHelper
    {
        /// 將字串轉成指定的Enum
        public static T StringToEnum<T>(this string value) where T : struct, Enum => Enum.Parse<T>(value, true);

        /// 取得指定Enum的內容，集成一個List字串
        public static List<string> EnumToList<T>() where T : struct, Enum => Enum.GetNames(typeof(T)).ToList();
    }
}