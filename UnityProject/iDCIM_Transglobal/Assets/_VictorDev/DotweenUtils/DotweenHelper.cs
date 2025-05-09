using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Random = UnityEngine.Random;

namespace VictorDev.DoTweenUtils
{
    public static class DotweenHelper
    {
        public static void ToBlink(List<TextMeshProUGUI> targets, string showText = null, float duration = 0.1f,
            float delay = 0, bool isRandomDelay = false)
            => targets.ForEach(txt => ToBlink(txt, showText, duration, delay, isRandomDelay));

        /// <summary>
        /// 閃爍後顯示指定文字
        /// <para>+ showText：顯示指定文字</para>
        /// <para>+ duration若太低，效果會不明顯</para>
        /// </summary>
        public static void ToBlink(TextMeshProUGUI target, string showText = null, float duration = 0.1f,
            float delay = 0, bool isRandomDelay = false)
        {
            DOTween.Kill(target);
            // 首先将Text的透明度设置为0（完全透明）
            target.DOFade(0f, duration).OnComplete(() =>
            {
                // 更改文本内容
                if (showText != null) target.SetText(showText);
                // 然后将Text的透明度从0渐变为1（完全不透明）
                target.DOFade(1f, duration).SetEase(Ease.OutQuad);
            }).SetDelay(Random.Range(isRandomDelay ? 0 : delay, delay));
        }

        public static Tween LerpValue(float startValue, float endValue, Action<float> onUpdate,
            float duration = 0.15f, float dealy = 0)
        {
            return DOTween.To(() => startValue, x =>
            {
                startValue = x;
                onUpdate.Invoke(startValue);
            }, endValue, duration).SetDelay(dealy).SetEase(Ease.OutQuad);
        }

        /// NEW===========================================================================================
        public static Tween DoInt(TextMeshProUGUI target, int startValue, int endValue, float duration = 1f,
            Ease ease = Ease.OutQuad)
        {
            return DOTween.To(() => startValue, x =>
            {
                startValue = x;
                target.SetText(startValue.ToString());
            }, endValue, duration).SetEase(ease);
        }

        public static Tween DoFloat(TextMeshProUGUI target, float startValue, float endValue, float duration = 1f,
            string formatter = "0.##", Ease ease = Ease.OutQuad)
        {
            return DOTween.To(() => startValue, x =>
            {
                startValue = x;
                target.SetText(startValue.ToString(formatter));
            }, endValue, duration).SetEase(ease);
        }
    }
}