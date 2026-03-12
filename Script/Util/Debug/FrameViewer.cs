using UnityEngine;
using UnityEngine.UI;

public class FrameViewer : MonoBehaviour
{
    [SerializeField] private Text sfText;
    private float mDeltaTime = 0f;

    public void Awake()
    {
        Debug.Log(Application.targetFrameRate);
    }
    void Update()
    {
        mDeltaTime += (Time.unscaledDeltaTime - mDeltaTime) * 0.1f;
        float ms = mDeltaTime * 1000f;
        float fps = 1.0f / mDeltaTime;
        string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);
        sfText.text = text;
    }
}