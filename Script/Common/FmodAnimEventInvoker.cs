using FMODUnity;
using UnityEngine;

public class FmodAnimEventInvoker : MonoBehaviour
{
    [SerializeField] private EventReference Sound;
    [SerializeField] private EventReference[] Sounds;

    public void Invoke()
    {
        Sound.TryPlay();
    }

    public void InvokeIndex(int index)
    {
        Sounds[index].TryPlay();
    }
}
