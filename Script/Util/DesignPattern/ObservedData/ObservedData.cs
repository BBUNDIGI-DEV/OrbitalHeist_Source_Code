using UnityEngine;

public struct ObservedData<T>
{
    public delegate void OnDataChange(T param);
    public delegate void OnDataChangeWithPrevValue(T prevValue, T param);

    public T Value
    {
        get
        {
            return mValue;
        }
        set
        {
            CallbackWithPrevValue?.Invoke(mValue, value);
            mValue = value;
            Callback?.Invoke(mValue);
            VoidCallback?.Invoke();
        }
    }

    public OnDataChange Callback
    {
        get; private set;
    }

    public OnDataChangeWithPrevValue CallbackWithPrevValue
    {
        get; private set;
    }

    public System.Action VoidCallback
    {
        get; private set;
    }

    private T mValue;

    public static implicit operator T(ObservedData<T> data)
    {
        return data.Value;
    }

    public void AddListener(OnDataChange callback, bool invokeOnAdd = false)
    {
        if (Callback == null)
        {
            Callback = callback;
        }
        else
        {
            Callback += callback;
        }

        if (invokeOnAdd)
        {
            callback.Invoke(mValue);
        }
    }
    public void AddListener(System.Action onValueChange, bool invokeOnAdd = false)
    {
        if (VoidCallback == null)
        {
            VoidCallback = onValueChange;
        }
        else
        {
            VoidCallback += onValueChange;
        }

        if (invokeOnAdd)
        {
            onValueChange.Invoke();
        }
    }
    public void AddListener(OnDataChangeWithPrevValue callback, bool inovkeOnAdd = false)
    {
        if (CallbackWithPrevValue == null)
        {
            CallbackWithPrevValue = callback;
        }
        else
        {
            CallbackWithPrevValue += callback;
        }

        if (inovkeOnAdd)
        {
            callback.Invoke(default(T), mValue);
        }
    }
    public void RemoveListener(OnDataChange callback)
    {
        Debug.Assert(Callback != null);
        Callback -= callback;
    }
    public void RemoveListener(System.Action onValueChange)
    {
        Debug.Assert(VoidCallback != null);
        VoidCallback -= onValueChange;
    }
    public void RemoveListener(OnDataChangeWithPrevValue onValueChange)
    {
        Debug.Assert(CallbackWithPrevValue != null);
        CallbackWithPrevValue -= onValueChange;
    }
    public void Clear()
    {
        mValue = default(T);
        Callback = null;
        CallbackWithPrevValue = null;
        VoidCallback = null;
    }
    public override string ToString()
    {
        return Value.ToString();
    }
}
