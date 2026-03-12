using UnityEngine;
using UnityEngine.Events;

    public class AnimEventReciever : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent[] TargetEvent;


        public void Raise(int eventIndex)
        {
            Debug.Assert(eventIndex < TargetEvent.Length, $"Anim Event Reciver Out Of Range, [{gameObject.name}]");
            TargetEvent[eventIndex].Invoke();
        }
    }
