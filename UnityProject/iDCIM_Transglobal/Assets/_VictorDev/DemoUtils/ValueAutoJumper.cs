using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace VictorDev.DemoUtils
{
    public class ValueAutoJumper : MonoBehaviour
    {
        public UnityEvent<int> onValueChangedInt;
        public UnityEvent<float> onValueChangedFloat;
        public UnityEvent<string> onValueChangedString;

        private void Start()
        {
            if (isActivatedInStart) StartJump();
        }

        [Button]
        public void StartJump()
        {
            IEnumerator JumpValue()
            {
                while (true)
                {
                    float value = Random.Range(minValue, maxValue);
                    float multiplier = Mathf.Pow(10f, afterDotNumber);

                    onValueChangedInt?.Invoke(Mathf.RoundToInt(value));
                    onValueChangedFloat?.Invoke(Mathf.Round(value * multiplier) / multiplier);
                    onValueChangedString?.Invoke(value.ToString($"F{afterDotNumber}"));
                    yield return new WaitForSeconds(intervalSec);
                }
            }

            _coroutine = StartCoroutine(JumpValue());
        }

        private void OnDisable()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
        }

        #region Variables

        [Foldout("[設定]")] [SerializeField] private bool isActivatedInStart = true;

        [Foldout("[設定]")] [Header("更新時間間隔")] [SerializeField]
        private float intervalSec = 5f;

        [Foldout("[設定]")] [Header("小數點後幾位")] [SerializeField]
        private int afterDotNumber = 2;

        [Foldout("[設定]")] [SerializeField] float minValue = 0f, maxValue = 100f;

        private Coroutine _coroutine;

        #endregion
    }
}