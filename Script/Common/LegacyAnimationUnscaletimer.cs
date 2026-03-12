using UnityEngine;

[RequireComponent(typeof(Animation))]
public class LegacyAnimationUnscaletimer : MonoBehaviour
{
    private Animation mAnim
    {
        get
        {
            if(_Anim == null)
            {
                _Anim = GetComponent<Animation>();
                if(_Anim.clip != null)
                {
                    mAnimState = _Anim[_Anim.clip.name];
                }
            }
            return _Anim;
        }
    }
    private AnimationState mAnimState;

    private Animation _Anim;
    public void Play(string key)
    {
        mAnim.Play(key);
        mAnimState = mAnim[key];
    }

    void Update()
    {
        if (!mAnim.isPlaying)
        {
            return;
        }

        // Manually advance the animation time using unscaledDeltaTime
        mAnimState.time += Time.unscaledDeltaTime;

        // Ensure the time wraps correctly for looping animations
        if (mAnimState.wrapMode == WrapMode.Loop)
        {
            mAnimState.normalizedTime = Mathf.Repeat(mAnimState.normalizedTime, 1f);
        }
        else if (mAnimState.wrapMode == WrapMode.ClampForever)
        {
            mAnimState.time = Mathf.Min(mAnimState.time, mAnimState.length);
        }

        // Sample the animation at the new time
        mAnim.Sample();
    }
}
