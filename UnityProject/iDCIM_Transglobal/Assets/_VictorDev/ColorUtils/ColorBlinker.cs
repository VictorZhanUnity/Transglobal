using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VictorDev.ColorUtils
{
    public class ColorBlinker : MonoBehaviour
    {
        [SerializeField] private bool isBlinkOnAwake = true;

        [Header(">>>正常時顏色")]
        [SerializeField] private Color normalColor = ColorHelper.HexToColor(0x74FF5A);

        [Header(">>>告警時閃爍顏色")]
        [SerializeField] public Color alarmColor1 = ColorHelper.HexToColor(0xFFAFAF);
        [SerializeField] public Color alarmColor2 = Color.red;

        [Header(">>> 目標組件(Image/TextMeshProUGUI)，若null則抓本身的組件")]
        [SerializeField] private Graphic target;

        public float duration = 0.5f;
        public Ease ease = Ease.InOutSine;

        private Tween blink { get; set; }

        private void OnEnable()
        {
            if (target == null)
            {
                if (target.TryGetComponent(out Image img)) target = img;
                else if (target.TryGetComponent(out TextMeshProUGUI txt)) target = txt;
            }
            SetBlinkStatus(isBlinkOnAwake);
        }

        private void OnDisable()
        {
            blink?.Kill();
        }

        public void SetBlinkStatus(bool isBlink)
        {
            if (isBlink) ToBlink();
            else ToNormal();
        }
        
        [ContextMenu("ToBlink")]
        public void ToBlink()
        {
            blink?.Kill();
            blink = ColorHelper.ToBlink(target, alarmColor1, alarmColor2, duration, ease);
        }

        [ContextMenu("ToNormal")]
        public void ToNormal()
        {
            blink?.Kill();
            blink = target.DOColor(normalColor, duration).SetEase(Ease.OutQuad);
        }
    }
}
