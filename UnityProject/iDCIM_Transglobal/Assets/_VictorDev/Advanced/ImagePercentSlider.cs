using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VictorDev.ColorUtils;

namespace VictorDev.Advanced
{
    [ExecuteInEditMode]
    public class ImagePercentSlider : MonoBehaviour
    {
        public void SetCurrentValue(int value)
        {
            currentValue = value;
            InvokeEvent();
        }

        public void SetMaxValue(int value)
        {
            maxValue = value;
            InvokeEvent();
        }

        void Update() => CaculateImagePercent();

        private void CaculateImagePercent()
        {
            if (targetImage == null) return;
            currentValue = Mathf.Clamp(currentValue, 0, maxValue);
            _percentValue = (float)currentValue / maxValue;

            if (Mathf.Abs(targetImage.fillAmount - _percentValue) > _threshold)
            {
                Color targetColor = colorSetting.First(level => _percentValue< level.percent01).color;
                if (Application.isPlaying)
                {
                    targetImage.DOFillAmount(_percentValue, 1f).SetEase(Ease.OutQuad);
                    targetImage.DOColor(targetColor, 1f).SetEase(Ease.OutQuad);
                }
                else
                {
                    targetImage.fillAmount = _percentValue;
                    targetImage.color = targetColor;
                }
                InvokeEvent();
            }
        }

        private void InvokeEvent()
        {
            onImagePercentChanged?.Invoke((_percentValue * 100f).ToString($"F{dotNumber}"));
            onInvokeCurrentValue?.Invoke(currentValue.ToString("N0"));
            onInvokeMaxValue?.Invoke(maxValue.ToString("N0"));
        }

        private void Start()
        {
            targetImage ??= GetComponent<Image>();
            InvokeEvent();
        }

        #region Variables
        [Header("[目前值]")] [SerializeField] private int currentValue;
        public UnityEvent<string> onImagePercentChanged = new();
        [Foldout("[其次事件]")] public UnityEvent<string> onInvokeCurrentValue = new();
        [Foldout("[其次事件]")] public UnityEvent<string> onInvokeMaxValue = new();
        
        [Foldout("[設定]")] [SerializeField] private int maxValue = 100;
        [Foldout("[設定]")] [SerializeField] private Image targetImage;
        [Foldout("[設定]")] [Header("小數點後幾位數")] [SerializeField] private int dotNumber = 2;

        [Header("[顏色等級設定]")] [SerializeField] private List<ColorHelper.ColorLevel> colorSetting = new ()
        {
            new ColorHelper.ColorLevel(0.3f, Color.red),
            new ColorHelper.ColorLevel(0.6f, ColorHelper.HexToColor(0xFFAF00)),
            new ColorHelper.ColorLevel(1f, Color.green),
        };
        private float _percentValue;
        private readonly float _threshold = 0.0001f;
        #endregion
        
    }
}