using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UnityEngine.UI
{
    public class MobileButton : MonoBehaviour, IPointerEnterHandler
    {
        public string Usage
        {
            get
            {
                return sfUsage;
            }
        }

        private Action<InputAction.CallbackContext> mCallback;
        [SerializeField] private string sfUsage;

        public void AddInputCallback(Action<InputAction.CallbackContext> callback)
        {
            mCallback += callback;
        }

        public void RemoveInputCallback(Action<InputAction.CallbackContext> callback)
        {
            mCallback -= callback;
        }

        public void ClearCallback()
        {
            mCallback = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mCallback?.Invoke(default(InputAction.CallbackContext));
        }
    }
}
