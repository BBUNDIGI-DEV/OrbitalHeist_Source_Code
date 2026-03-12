using FMODUnity;

public static class EventReferenceExtenstion
{
    public static void TryPlay(this EventReference eventRefernce)
    {
        if(!eventRefernce.IsNull)
        {
            RuntimeManager.PlayOneShot(eventRefernce);
        }
    }
}
