#if UNITY_EDITOR

using Sirenix.OdinInspector;
using Sirenix.Utilities; 
using UnityEngine;

[GlobalConfig("Assets/Resources/SkillGlobalConfig/")]
public class GlobalSkillConfig : GlobalConfig<GlobalSkillConfig>
{
    [AssetsOnly] public ParticleBinder DefaultNockbackFX;

    public AnimationCurve RecoilCameraCurve;
    public AnimationCurve BumpCameraCurve;
    public AnimationCurve ExplosionCameraCurve;
    public AnimationCurve RumbleCameraCurve;
    public AnimationCurve ImpactCamerCurve;
}
#endif