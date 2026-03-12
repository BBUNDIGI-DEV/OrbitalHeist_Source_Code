//using UnityEngine;
//using UnityEngine.SceneManagement;
//public class RoboBossMonsterController : MonsterBase
//{
//    protected override void Awake()
//    {
//        base.Awake();
//        Status.IsSuperArmor.Value = true;
//    }

//    private void Start()
//    {
//        AudioManager.PlayBossIdleSound();
//        Status.NormalizedHP.AddListener((hp) =>
//        {
//            if (hp < 0.5f)
//            {
//                BGMManager.Instance.SetBGMLabel("BossHP50%");
//            }
//            else
//            {
//                BGMManager.Instance.SetBGMLabel("Boss");
//            }
//        }, true);
//    }

//    private void OnDestroy()
//    {
//        AudioManager.ReleaseBossIdleSound();
//    }

//    public void GotStunedByRush(HitEffectData hitEffectData)
//    {
//        mStateMachine.GetActor<RoboBossAISkillActor>(eActorType.AIAttack).SetRushHitObastacleFlag();
//        mStateMachine.TrySwitchActor(eActorType.Damaged, hitEffectData, null);
//    }

//    public void DoRushComboAttack()
//    {
//        mStateMachine.GetActor<RoboBossAISkillActor>(eActorType.AIAttack).SetRushHitPlayerFlag();
//    }
//}