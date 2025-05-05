using System;
using System.Collections;
using UnityEngine;
using VictorDev.Common;

namespace VictorDev.Async.CoroutineUtils
{
    /// <summary>
    /// 特地獨立出來進行Coroutine相關處理
    /// <para>+ 為了避免GameObject被Disabled時無法執行Coroutine的問題</para>
    /// </summary>
    public class CoroutineHandler : SingletonMonoBehaviour<CoroutineHandler>
    {
        /// <summary>
        /// 執行Coroutine
        /// </summary>
        public static Coroutine ToStartCoroutine(IEnumerator iEnumerator) => Instance.StartCoroutine(iEnumerator);

        /// <summary>
        /// 取消Coroutine
        /// </summary>
        public static void ToStopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null) Instance.StopCoroutine(coroutine);
        }

        /// <summary>
        /// 等侯幾秒後執行Action
        /// </summary>
        public static Coroutine HoldAndRun(Action action, float delaySec = 3)
        {
            IEnumerator enumerator()
            {
                yield return new WaitForSeconds(delaySec);
                action.Invoke();
            };
            return ToStartCoroutine(enumerator());
        }

        //=======================================================

        /// <summary>
        /// 執行Coroutine (舊版)
        /// </summary>
        public static IEnumerator RunCoroutine_Old(IEnumerator iEnumerator)
        {
            Instance.StartCoroutine(iEnumerator);
            return iEnumerator;
        }

        /// <summary>
        /// 取消Coroutine(舊版)
        /// </summary>
        public static void CancellCoroutine_Old(IEnumerator iEnumerator) => Instance.StopCoroutine(iEnumerator);
    }
}
