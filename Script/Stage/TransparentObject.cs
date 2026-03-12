using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    [SerializeField] private GameObject sfTransparentObject;
    [SerializeField] private bool sfIsCompleleteFadeObject;

    private CameraConfig CONFIG
    {
        get
        {
            return RuntimeDataLoader.CameraConfig;
        }
    }
    private List<Material> mTransparentMats;

    private float mOpacityValue;

    private bool mIsTransparent;

    private void Awake()
    {
        mOpacityValue = 1f;
        mIsTransparent = false;
        if(sfTransparentObject == null)
        {
            sfTransparentObject = gameObject;
        }

        Renderer[] renderer = sfTransparentObject.GetComponentsInChildren<Renderer>();

        mTransparentMats = new List<Material>();
        for (int i = 0; i < renderer.Length; i++)
        {
            Material[] mats = renderer[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                if (mats[j].shader.name.Contains("lit_Dithering"))
                {
                    mTransparentMats.Add(mats[j]);
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            mIsTransparent = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            mIsTransparent = false;
        }
    }

    private void Update()
    {
        if (mIsTransparent)
        {
            fadeOutObject();
        }
        else if (!mIsTransparent)
        {
            fadeInObject();
        }

        fadeUpdateObject();
    }


    private void fadeInObject()
    {
        if (mOpacityValue >= 1f)
        {
            mOpacityValue = 1f;
            return;
        }
        mOpacityValue = mOpacityValue + (CONFIG.FadeInSpeed * Time.deltaTime);
    }

    private void fadeOutObject()
    {
        if (!sfIsCompleleteFadeObject)
        {
            if (mOpacityValue <= 0.5f)
            {
                mOpacityValue = 0.5f;
                return;
            }
        }
        else if (sfIsCompleleteFadeObject)
        {
            if (mOpacityValue <= 0f)
            {
                mOpacityValue = 0f;
                return;
            }
        }
        mOpacityValue = mOpacityValue - (CONFIG.FadeOutSpeed * Time.deltaTime);
    }

    private void fadeUpdateObject()
    {

        for (int i = 0; i < mTransparentMats.Count; i++)
        {
            if(mOpacityValue >= 0.95f)
            {
                mTransparentMats[i].SetFloat("_DitherSize", 0.0f);
            }
            else
            {
                mTransparentMats[i].SetFloat("_DitherSize", 1.0f);
            }
            mTransparentMats[i].SetFloat("_Opacity", mOpacityValue);
        }
    }
}
