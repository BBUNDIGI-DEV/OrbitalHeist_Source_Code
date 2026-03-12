namespace UnityEngine.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIRenderCameraSetter : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Canvas>().worldCamera = CameraManager.Instance.UICamera;
        }
    }
}
