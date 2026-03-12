using UnityEngine;
using UnityEngine.Events;

    public class TriggerEventReciever : MonoBehaviour
    {
        [SerializeField] private bool sfAutoDisable = true;
        [SerializeField] private string sfTag;
        [SerializeField] private UnityEvent sfEvent;
        public void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag(sfTag))
            {
                return;
            }

            if (sfAutoDisable)
            {
                GetComponent<Collider>().enabled = false;
            }

            sfEvent.Invoke();

        }
    }
