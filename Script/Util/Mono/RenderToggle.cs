using UnityEngine;

public class RenderToggle : MonoBehaviour
{
    private Renderer[] mCahsedRenderers;
    private SpriteRenderer[] mCashedSR;

    public void Toggle(bool enabled)
    {
        if (mCahsedRenderers == null)
        {
            mCahsedRenderers = GetComponentsInChildren<Renderer>(true);
            mCashedSR = GetComponentsInChildren<SpriteRenderer>(true);
        }


        for (int i = 0; i < mCahsedRenderers.Length; i++)
        {
            mCahsedRenderers[i].enabled = enabled;
        }

        for (int i = 0; i < mCashedSR.Length; i++)
        {
            mCashedSR[i].enabled = enabled;
        }
    }
}
