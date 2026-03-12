using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class SerializableObesrvedData<T>
{
    public T Value
    {
        get
        {
            return mObservedData.Value;
        }
        set
        {
            mObservedData.Value = value;
        }
    }

    private ObservedData<T> mObservedData;
#if UNITY_EDITOR
    [HideInEditorMode, ShowInInspector]
    private T mCurrentValue
    {
        get
        {
            return Value;
        }
    }

    [HideInEditorMode, SerializeField] private T sfUpdateVlaue;

    [HideInEditorMode, Button]
    private void UpdateValue()
    {
        Value = sfUpdateVlaue;
    }
#endif

    public static implicit operator T(SerializableObesrvedData<T> data)
    {
        return data.Value;
    }


    public void AddListener(ObservedData<T>.OnDataChange onValueChange, bool invokeOnAdd = false)
    {
        mObservedData.AddListener(onValueChange, invokeOnAdd);
    }

    public void AddListener(ObservedData<T>.OnDataChangeWithPrevValue onValueChange, bool invokeOnAdd = false)
    {
        mObservedData.AddListener(onValueChange, invokeOnAdd);
    }

    public void AddListener(System.Action onValueChange, bool invokeOnAdd = false)
    {
        mObservedData.AddListener(onValueChange, invokeOnAdd);
    }

    public void RemoveListener(ObservedData<T>.OnDataChange onValueChange)
    {
        mObservedData.RemoveListener(onValueChange);
    }

    public void RemoveListener(ObservedData<T>.OnDataChangeWithPrevValue onValueChange)
    {
        mObservedData.RemoveListener(onValueChange);
    }

    public void RemoveListener(System.Action onValueChange)
    {
        mObservedData.RemoveListener(onValueChange);
    }

    public void Clear()
    {
        mObservedData.Clear();
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

[System.Serializable]
public class FloatObservedData : SerializableObesrvedData<float>
{

}

[System.Serializable]
public class BoolObservedData : SerializableObesrvedData<bool>
{

}

[System.Serializable]
public class InteractableTypeObservedData : SerializableObesrvedData<eInteractableType>
{

}