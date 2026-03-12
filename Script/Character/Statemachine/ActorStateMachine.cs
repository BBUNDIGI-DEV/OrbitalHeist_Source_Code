using System.Collections.Generic;
using UnityEngine;

public class ActorStateMachine
{
    private struct ActorIncokingBufferingData
    {
        public eActorType BufferedActor;
        public object BufferedParam1;
        public object BufferedParam2;
        public bool BufferedNeedPausePrevActorOption;

        public ActorIncokingBufferingData(eActorType bufferedActor, object bufferedParam1, object bufferedParam2, bool bufferedNeedPausePrevActorOption) : this()
        {
            Set(bufferedActor, bufferedParam1, bufferedParam2, bufferedNeedPausePrevActorOption);
        }

        public void Set(eActorType bufferedActor, object bufferedParam1, object bufferedParam2, bool bufferedNeedPausePrevActorOption)
        {
            BufferedActor = bufferedActor;
            BufferedParam1 = bufferedParam1;
            BufferedParam2 = bufferedParam2;
            BufferedNeedPausePrevActorOption = bufferedNeedPausePrevActorOption;
        }

        public void Clear()
        {
            BufferedActor = eActorType.None;
            BufferedParam1 = null;
            BufferedParam2 = null;
            BufferedNeedPausePrevActorOption = false;
        }
    }

    public eActorType CurrentActorType
    {
        get
        {
            return CurrentActorOrNull == null ? eActorType.None : CurrentActorOrNull.ActorType;
        }
    }
    public ActorBase CurrentActorOrNull
    {
        get; private set;
    }

    public CharacterBase OwnerCharacterBase
    {
        get; private set;
    }

    public TranslatorBinder Translator
    {
        get
        {
            return OwnerCharacterBase.Translator;
        }
    }
    public CharacterAnimator Anim
    {
        get
        {
            return OwnerCharacterBase.Anim;
        }
    }
    public bool IsPlayerActor
    {
        get; private set;
    }
    private ActorIncokingBufferingData mBufferedData;
    private List<ActorBase> mUpdatedActors;
    private Dictionary<eActorType, ActorBase> mActorDic;
    private bool mIsCancleOperation;

    public ActorStateMachine(CharacterBase owner, bool isPlayerActor)
    {
        mActorDic = new Dictionary<eActorType, ActorBase>();
        mUpdatedActors = new List<ActorBase>();
        mBufferedData.BufferedActor = eActorType.None;
        IsPlayerActor = isPlayerActor;
        OwnerCharacterBase = owner;
    }

    public void AddActor(ActorBase actor)
    {
        Debug.Assert(!mActorDic.ContainsKey(actor.ActorType),
            $"Duplicate Actor in ActorStateMachine [{actor.ActorType}]");
        mActorDic.Add(actor.ActorType, actor);

        if(actor.BaseConfig.IsUpdatedActor)
        {
            mUpdatedActors.Add(actor);
        }
    }

    public void RemoveUpdatedActor(eActorType actorType)
    {
        for (int i = 0; i < mUpdatedActors.Count; i++)
        {
            if(mUpdatedActors[i].ActorType == actorType)
            {
                mUpdatedActors.RemoveAt(i);
                return;
            }
        }
        Debug.LogError($"Cannot found actor in statemachine {actorType}");
    }

    public void SetEnabledUpdate(bool enabled, eActorType actor)
    {
        for (int i = 0; i < mUpdatedActors.Count; i++)
        {
            if(mUpdatedActors[i].ActorType == actor)
            {
                mUpdatedActors[i].SetEnabledUpdating(enabled);
                return;
            }
        }
        Debug.LogError($"Cannot found updated actor in updatedActors List {actor}");
    }

    public bool HasActor(eActorType type)
    {
        return mActorDic.ContainsKey(type);
    }

    public T GetActor<T>(eActorType actorType) where T : ActorBase
    {
        Debug.Assert(mActorDic.ContainsKey(actorType), $"[{actorType}] is not contained");
        return mActorDic[actorType] as T;
    }

    public T TryGetActorOrNull<T>(eActorType actorType) where T : ActorBase
    {
        if(!mActorDic.ContainsKey(actorType))
        {
            return null;
        }
        return mActorDic[actorType] as T;
    }

    public void UpdateActor(float deltaTime)
    {
        for (int i = 0; i < mUpdatedActors.Count; i++)
        {
            if(mUpdatedActors[i].NeedUpdate)
            {
                mUpdatedActors[i].UpdateActing(deltaTime);
            }
        }
    }

    public void CheckAndClearActor(eActorType checkType)
    {
        if(CurrentActorType != checkType)
        {
            return;
        }
        clearActor();
    }

    public void CheckAndClearActor(string nameID)
    {
        if (CurrentActorOrNull == null || CurrentActorOrNull.NameID != nameID)
        {
            return;
        }
        clearActor();
    }

    public void TryInvokeBufferedActor()
    {
        if (mBufferedData.BufferedActor != eActorType.None)
        {
            TrySwitchActor(mBufferedData.BufferedActor, mBufferedData.BufferedParam1, mBufferedData.BufferedParam2, mBufferedData.BufferedNeedPausePrevActorOption);
            mBufferedData.Clear();
        }
    }

    public bool TrySwitchActor(eActorType actorType)
    {
        return TrySwitchActor(actorType, null, null, true);
    }

    public bool TrySwitchActor(eActorType actorType, bool needPausePrevActor)
    {
        return TrySwitchActor(actorType, null, null, needPausePrevActor);
    }

    public bool TrySwitchActor(eActorType actorType, object paramter1, object paramter2)
    {
        return TrySwitchActor(actorType, paramter1, paramter2, true);
    }

    public bool TrySwitchActor(eActorType actorType, object paramter1, object paramter2, bool needPausePrevActor)
    {
        Debug.Assert(mActorDic.ContainsKey(actorType), $"You Cannot switch to actor not added in ActorStateMachine [{actorType}]");

        //해당 플래그를 통해 캔슬도중 의도치 않은 Actor변경을 방지합니다.
        //예를 들어 공격이 끝나면 Idle 모션으로 자동으로 이동합니다.
        //다만 공격을 캔슬하고 대쉬를 실행하는 상황을 가정한다면, 공격이 멈추는 로직상 자동적으로 Idle모션으로 변경을 시도합니다
        //하지만 현재 대쉬로 캔슬되는 과정이기때문에(mIsCancleOperation == true) 해당 플래그를 통해 공격의 정지와 동시에 Idle 이동하려는 동작은
        //아무런 처리 없이 return합니다.
        if (mIsCancleOperation == true)
        {
            return false;
        }

        if(CurrentActorType == eActorType.Dead)
        {
            return false;
        }

        mIsCancleOperation = true;
        ActorBase switchTarget = mActorDic[actorType];
        string debugString = CurrentActorOrNull == null ? "Null" : CurrentActorOrNull.NameID;
        bool canSwitchable = checkSwitchable(actorType);

        if (!canSwitchable && mBufferedData.BufferedActor != eActorType.UltimateAttack) // You Cannot buffering action if current buffering action is UltiamteSkill
        {
            mBufferedData.Set(actorType, paramter1, paramter2, needPausePrevActor);

            //Damaged는 버퍼에 넣지 않습니다 즉 캔슬불가능한 상황에서 들어오는 딜은 데미지만 들어가고 엑터는 생략됩니다.
            goto returnState;
        }

        if (actorType == eActorType.Dead)
        {
            PauseAllActorWithoutDead();
        }
        else if (CurrentActorOrNull != null && needPausePrevActor)
        {
            CurrentActorOrNull.StopActing();
        }

        switchTarget.InovkeActing(paramter1, paramter2);
            CurrentActorOrNull = switchTarget;

     returnState:
        mIsCancleOperation = false;
        return true;
    }

    public void DestoryActors()
    {
        foreach (var item in mActorDic)
        {
            item.Value.DestoryActor();
        }
    }

    public void PauseAllActorWithoutDead()
    {
        foreach (var item in mActorDic)
        {
            if(item.Value.ActorType == eActorType.Dead)
            {
                continue;
            }

            item.Value.StopActing();
        }
    }

    public void PauseAllActor()
    {
        foreach (var item in mActorDic)
        {
            item.Value.StopActing();
        }
    }

    private bool checkSwitchable(eActorType newActor)
    {
        if (CurrentActorOrNull == null)
        {
            return true;
        }

        if (newActor == eActorType.Damaged || newActor == eActorType.Dead)
        {
            return true;
        }

        if (CurrentActorType == eActorType.Damaged)
        {
            return false;
        }


        if (CurrentActorType == eActorType.Appearance)
        {
            return false;
        }


        if (CurrentActorType == eActorType.TryCounterAttack && newActor == eActorType.CounterAttack)
        {
            return true;
        }

        if(IsPlayerActor)
        {
            if (CurrentActorType.IsAttackType())
            {
                SkillActor skillActor = CurrentActorOrNull as SkillActor;
                
                if(skillActor.ActorType == eActorType.UltimateAttack && skillActor.CurrentProgressState == eSkillProgressState.Preparing)
                {
                    return false;
                }
                if (skillActor.CurrentProgressState == eSkillProgressState.OnAttack)
                {
                    return false;
                }
                else if (skillActor.SkillConfig.IsComboAttack
                    && (skillActor.BaseConfig.ActorType == newActor)
                    && skillActor.CurrentProgressState != eSkillProgressState.Cancelable
                    && (skillActor as ComboSkillActor).IsWaitComboAttackInput)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void clearActor()
    {
        CurrentActorOrNull = null;
        TryInvokeBufferedActor();
    }
}

