using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace VictorDev.TextUtils
{
    public static class TextHelper
    {
        public static void EventCheckIsInputHaveValue(List<TMP_InputField> inputFields, Action<bool> onCheckHandler)
            => inputFields.ForEach(target 
                => target.onValueChanged.AddListener(_=> onCheckHandler?.Invoke(IsInputHaveValue(inputFields))));

        public static bool IsInputHaveValue(List<TMP_InputField> inputFields)
            => inputFields.All(target => string.IsNullOrEmpty(target.text.Trim())) == false;
    }
}