using UnityEngine;
using Cinemachine;
public class StageVCamSetter : MonoBehaviour
{
    private CinemachineVirtualCamera mVCam;
    private void Awake()
    {
        mVCam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if(!mVCam.enabled)
        {
            return;
        }

        if(mVCam.Follow == PlayerManager.Instance.AcitvatedPlayerTrans)
        {
            return;
        }

        mVCam.Follow = PlayerManager.Instance.AcitvatedPlayerTrans;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        mVCam.Follow = PlayerManager.Instance.AcitvatedPlayerTrans;
        mVCam.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        mVCam.enabled = false;

    }
}
