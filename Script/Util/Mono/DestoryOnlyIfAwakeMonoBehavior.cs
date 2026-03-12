using UnityEngine;

    public abstract class DestoryOnlyIfAwakeMonoBehavior : MonoBehaviour
    {
        protected bool mIsAwaked = false;

        protected virtual void Awake()
        {
            mIsAwaked = true;
        }

        private void OnDestroy()
        {
            if (!mIsAwaked)
            {
                return;
            }
            onDestory();
        }

        private void OnDisable()
        {
            if (!mIsAwaked)
            {
                return;
            }
            onDisable();
        }

        protected abstract void onDisable();

        protected abstract void onDestory();
    }
