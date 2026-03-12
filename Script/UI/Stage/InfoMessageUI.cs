using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;

namespace UnityEngine.UI
{
    public class InfoMessageUI : MonoBehaviour
    {
        private const string M_SHOW_UP_ANIM_TRIGGER = "ShowUp";
        private const string M_CLOSE_ANIM_TRIGGER = "Close";
        private const float M_CLOSE_ANIM_DURATION = 1.0f;
        [SerializeField] private Animator sfInfoMessageAnimator;
        [SerializeField] private TypewriterCore sfTextAnimator;
        [SerializeField] private InfoMessageUIConfig[] sfConfigs;

        private Queue<string> mEnququedMessages;
        private InfoMessageUIConfig mCurrentConfig;
        private Coroutine mInfoMessageRoutineOrNull;
        private float mLastToxic;
        private float mMaxToxicReached;
        private int mIncreaseIndex;
        private int mDecreaseIndex;

        private void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            mEnququedMessages = new Queue<string>();
        }

        private void Start()
        {
            RuntimeDataLoader.RuntimeStageData.IsStageStarted.AddListener(onStageStarted);
            RuntimeDataLoader.RuntimeStageData.IsStageCleared.AddListener(onStageCleared);
            SceneSwitchingManager.Instance.CurrentScene.AddListener(onSceneChange, true);
        }

        private void OnDestroy()
        {
            RuntimeDataLoader.RuntimeStageData.IsStageStarted.RemoveListener(onStageStarted);
            RuntimeDataLoader.RuntimeStageData.IsStageCleared.RemoveListener(onStageCleared);

            if (SceneSwitchingManager.IsExist)
            {
                SceneSwitchingManager.Instance.CurrentScene.RemoveListener(onSceneChange);
            }
        }


        public void resetData()
        {
            eSceneName name = SceneSwitchingManager.Instance.CurrentScene;
            if (!name.CheckIsLevelScene())
            {
                return;
            }

            mCurrentConfig = sfConfigs[(int)name - (int)eSceneName.Scene_Stage2];
            mLastToxic = 0.0f;
            mMaxToxicReached = 0.0f;

            mIncreaseIndex = 0;
            mDecreaseIndex = -1;
            mEnququedMessages.Clear();
            if (mInfoMessageRoutineOrNull != null)
            {
                StopCoroutine(mInfoMessageRoutineOrNull);
                mInfoMessageRoutineOrNull = null;
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }


        public void EnqueueMessage(string text)
        {
            mEnququedMessages.Enqueue(text);

            if (mEnququedMessages.Count == 1 && mInfoMessageRoutineOrNull == null)
            {
                showHelpMessage(mEnququedMessages.Dequeue());
            }
        }
        private void showHelpMessage(string text)
        {
            mInfoMessageRoutineOrNull = StartCoroutine(showUpTextRoutine(text));
        }

        private IEnumerator showUpTextRoutine(string text)
        {
            transform.GetChild(0).gameObject.SetActive(true);

            sfTextAnimator.ShowText(text);
            sfInfoMessageAnimator.SetTrigger(M_SHOW_UP_ANIM_TRIGGER);

            float remaningTime = mCurrentConfig.RemaningDuration;
            while (remaningTime > 0.0f)
            {
                if (mEnququedMessages.Count != 0)
                {
                    yield return new WaitForSeconds(2.0f);
                    sfTextAnimator.ShowText(mEnququedMessages.Dequeue());
                    remaningTime = mCurrentConfig.RemaningDuration;
                }
                remaningTime -= Time.deltaTime;
                yield return null;
            }

            sfInfoMessageAnimator.SetTrigger(M_CLOSE_ANIM_TRIGGER);
            yield return new WaitForSeconds(M_CLOSE_ANIM_DURATION);

            transform.GetChild(0).gameObject.SetActive(false);
            mInfoMessageRoutineOrNull = null;
            if (mEnququedMessages.Count != 0)
            {
                showHelpMessage(mEnququedMessages.Dequeue());
            }
        }

        private void onToxicChange(float newToxic)
        {
            float deltaToxic = newToxic - mLastToxic;
            mLastToxic = newToxic;
            if (mLastToxic > mMaxToxicReached)
            {
                mMaxToxicReached = mLastToxic;

                if (mDecreaseIndex + 1 < mCurrentConfig.DecreaseMessageMaxThreshold.Length &&
                    mMaxToxicReached > mCurrentConfig.DecreaseMessageMaxThreshold[mDecreaseIndex + 1])
                {
                    mDecreaseIndex++;
                }
            }

            if (deltaToxic > 0.0f)
            {
                if (mIncreaseIndex == mCurrentConfig.IncreaseMessageInvokeThreshold.Length)
                {
                    return;
                }
                float increaseThreshold = mCurrentConfig.IncreaseMessageInvokeThreshold[mIncreaseIndex];
                if (mLastToxic < increaseThreshold)
                {
                    return;
                }

                EnqueueMessage(mCurrentConfig.IncreaseInfoMessage[mIncreaseIndex]);
                mIncreaseIndex++;
            }
            else
            {
                if (mDecreaseIndex == -1)
                {
                    return;
                }

                float decreaseThreshold = mCurrentConfig.DecreaseMessageInvokeThreshold[mDecreaseIndex];
                if (mLastToxic > decreaseThreshold)
                {
                    return;
                }

                EnqueueMessage(mCurrentConfig.DecreaseInfoMessage[mDecreaseIndex]);
                mDecreaseIndex--;
            }
        }

        private void onSceneChange()
        {
            resetData();
        }

        private void onStageStarted(bool isStarted)
        {
            if (!isStarted)
            {
                return;
            }
            if (mCurrentConfig.InvokeTiming.HasFlag(eInfoMessageInvokeTiming.OnStageStarted))
            {
                EnqueueMessage(mCurrentConfig.StageStartMessage);
            }
        }

        private void onStageCleared(bool iscleared)
        {
            if (!iscleared)
            {
                return;
            }

            mEnququedMessages.Clear();
            if (mCurrentConfig.InvokeTiming.HasFlag(eInfoMessageInvokeTiming.OnStageCleared))
            {
                EnqueueMessage(mCurrentConfig.StageClearMessage);
            }
        }
    }
}