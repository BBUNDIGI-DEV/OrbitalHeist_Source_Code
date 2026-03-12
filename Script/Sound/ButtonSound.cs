using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    private Button mButton;

    private void Awake()
    {
        mButton = GetComponent<Button>();
    }

    private void Start()
    {
        if (mButton != null)
        {
            mButton.onClick.AddListener(playSound);
        }
    }

    private void playSound()
    {
    }

    public void PlayUIButtonOnEnableSoundEffect()
    {
        RuntimeManager.PlayOneShot("event:/SFX/UI/SFX_ButtonHighlighted");
    }
}