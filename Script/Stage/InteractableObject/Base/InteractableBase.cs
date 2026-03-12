using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public abstract class InteractableBase : MonoBehaviour
{
    public eInteractableType ObjectType
    {
        get
        {
            return sfObjetType;
        }
    }

    [SerializeField] private eInteractableType sfObjetType;
    [SerializeField] private UnityEvent sfOnIntercting;
    [SerializeField] private UnityEvent sfOnInteractingEnd;
    [SerializeField] private bool sfDeactiveOnInteractingEnd;
    private GameObject mIndicatorUI
    {
        get
        {
            if(_IndicatorUI == null)
            {
                _IndicatorUI = transform.FindRecursiveGameobjectWithTag("InteractableIndicator");
            }
            return _IndicatorUI;
        }
    }
    private GameObject _IndicatorUI;
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }
        Debug.Log(other.gameObject);
        PlayerInteractiveManager interactiveManager = other.GetComponentInParent<PlayerInteractiveManager>();
        Debug.Assert(interactiveManager != null,
            "interactive Manager Not Founded" );
        interactiveManager.SetDetectedObject(this);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        PlayerInteractiveManager interactiveManager = other.GetComponentInParent<PlayerInteractiveManager>();
        Debug.Assert(interactiveManager != null,
            "interactive Manager Not Founded");
        interactiveManager.SetDetectedObject(null);
    }

    public virtual void InvokeInteraction()
    {
        GetComponent<Collider>().enabled = false;
        sfOnIntercting?.Invoke();
    }

    public virtual void DisableInteractable()
    {
        ToggleInteractableObject(false);
        sfOnInteractingEnd?.Invoke();
    }

    public void ToggleInteractableObject(bool toggle)
    {
        if(toggle == false)
        {
            mIndicatorUI.gameObject.SetActive(false);
            GetComponent<Collider>().enabled = false;
        }
        else
        {
            mIndicatorUI.gameObject.SetActive(true);
            GetComponent<Collider>().enabled = true;
        }
    }
}


public enum eInteractableType
{
    None,
    GrowthItem,
    DialogueNPC,
    CommonObject,
    EndingObject,
    Stage1Door,
    Stage2Door,
    Stage3BossBattle,
}
