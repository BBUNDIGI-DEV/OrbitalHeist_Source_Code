
namespace UnityEngine.UI
{
    public class UICameraBilloboard : PoolableMono
    {
        private void LateUpdate()
        {
            if (CameraManager.IsExist)
            {
                transform.rotation = CameraManager.Instance.MainCamera.transform.rotation;
            }
        }
    }
}
