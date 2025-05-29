using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VictorDev.ColorUtils;
using VictorDev.DoTweenUtils;

namespace VictorDev.Advanced
{
    public class ImagePercentSlider : MonoBehaviour
    {
        public void SetCurrentValue(int value)
        {
            currentValue = value;
            CaculateImagePercent();
        }

        public void SetMaxValue(int value)
        {
            maxValue = value;
            CaculateImagePercent();
        }

        public void AddValue(int value) => SetCurrentValue(currentValue + value);


        private void CaculateImagePercent()
        {
            if (targetImage == null) return;
            currentValue = Mathf.Clamp(currentValue, 0, maxValue);
            _percentValue = (float)currentValue / maxValue;

            if (Mathf.Abs(targetImage.fillAmount - _percentValue) > _threshold)
            {
                _levelColor = colorSetting.First(level => _percentValue< level.percent01).color;
                /*if (Application.isPlaying)
                {
                    targetImage.DOFillAmount(_percentValue, 1f).SetEase(Ease.OutQuad).From(0);
                    targetImage.DOColor(_levelColor, 1f).SetEase(Ease.OutQuad).From(new Color(0,0,0,0));
                }
                else
                {
                    targetImage.fillAmount = _percentValue;
                    targetImage.color = _levelColor;
                }*/
                UpdateUI();
            }
        }

        private void UpdateUI(bool isOnEanbeld=false)
        {
            DotweenHelper.DoInt(TxtCurrentValue, isOnEanbeld? 0 : int.Parse(TxtCurrentValue.text.Replace(",", "")), currentValue);
            DotweenHelper.DoInt(TxtPercent, isOnEanbeld? 0 : int.Parse(TxtPercent.text.Replace(",", "")), (int)(_percentValue * 100f));
            targetImage.DOFillAmount(_percentValue, 1f).SetEase(Ease.OutQuad).From(isOnEanbeld? 0 : targetImage.fillAmount);
            targetImage.DOColor(_levelColor, 1f).SetEase(Ease.OutQuad).From(Color.red);
        }

        private void Start()
        {
            targetImage ??= GetComponent<Image>();
            TxtMaxValue.SetText(maxValue.ToString("N0"));
            UpdateUI();
        }

        private void OnEnable()
        {
            UpdateUI(true);
        }


        #region Variables
        [Header("[目前值]")] [SerializeField] private int currentValue;
        
        [Foldout("[設定]")] [SerializeField] private int maxValue = 100;
        [Foldout("[設定]")] [SerializeField] private Image targetImage;

        [Header("[顏色等級設定]")] [SerializeField] private List<ColorHelper.ColorLevel> colorSetting = new ()
        {
            new ColorHelper.ColorLevel(0.3f, Color.red),
            new ColorHelper.ColorLevel(0.6f, ColorHelper.HexToColor(0xFFAF00)),
            new ColorHelper.ColorLevel(1f, Color.green),
        };
        private float _percentValue;
        private readonly float _threshold = 0.0001f;
        private Color _levelColor;

        private TextMeshProUGUI TxtPercent =>_txtPercent ??= transform.Find("ImgProgress/TxtPercent").GetComponent<TextMeshProUGUI>();
        [NonSerialized]
        private TextMeshProUGUI _txtPercent;
        private TextMeshProUGUI TxtCurrentValue =>_txtCurrentValue ??= transform.Find("Panel/TxtValue").GetComponent<TextMeshProUGUI>();
        [NonSerialized]
        private TextMeshProUGUI _txtCurrentValue;
        private TextMeshProUGUI TxtMaxValue =>_txtMaxValue ??= transform.Find("Panel/TxtMaxValue").GetComponent<TextMeshProUGUI>();
        [NonSerialized]
        private TextMeshProUGUI _txtMaxValue;
        #endregion

    }
}