using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class SimpleEnumAnimator : MonoBehaviour
    {
        public Animator Animator
        {
            get;
            private set;
        }

        private int[] mAnimHashKeys;
        private string[] mAnimationKeys;

        public void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public void InitAnimPlayer(Type enumType)
        {
            mAnimationKeys = Enum.GetNames(enumType);
            mAnimHashKeys = new int[mAnimationKeys.Length];
            for (int i = 0; i < mAnimHashKeys.Length; i++)
            {
                mAnimHashKeys[i] = Animator.StringToHash(mAnimationKeys[i]);
            }
        }

        public void SetBool(int keyIndex, bool toggle)
        {
            Animator.SetBool(mAnimHashKeys[keyIndex], toggle);
        }

        public void SetTrigger(int keyIndex)
        {
            Animator.SetTrigger(mAnimHashKeys[keyIndex]);
        }

        public void SetFloat(int keyIndex, float value)
        {
            Animator.SetFloat(mAnimHashKeys[keyIndex], value);
        }

        public void Play(int keyIndex)
        {
            Animator.Play(mAnimationKeys[keyIndex]);
        }

        public void PlayByString(string state)
        {
            Animator.Play(state);
        }

        public bool CheckAnimState(string stateName, int layerIndex)
        {
            return Animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        }
    }
