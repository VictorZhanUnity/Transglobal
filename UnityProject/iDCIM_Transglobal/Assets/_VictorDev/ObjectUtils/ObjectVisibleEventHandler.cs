using UnityEngine;
using UnityEngine.Events;

namespace VictorDev.ObjectUtils
{
    public class ObjectVisibleEventHandler : MonoBehaviour
    {
        public UnityEvent<bool> isOnEvent = new();
        public UnityEvent showEvent = new();
        public UnityEvent hideEvent = new();

        public bool isVisible;

        public bool IsOn
        {
            set
            {
                isVisible = value;
                if (isVisible) ToShow();
                else ToHide();
            }
            get => isVisible;
        }

        private void Awake() => IsOn = isVisible;

        public void ToShow()
        {
            showEvent?.Invoke();
            isOnEvent?.Invoke(true);
        }

        public void ToHide()
        {
            hideEvent?.Invoke();
            isOnEvent?.Invoke(false);
        }
    }
}