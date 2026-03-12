# SkillConfig 설정 변수들

| Variable | Type | Description |
|---|---|---|
| SkillActorType |[eActorType](#열거형-eactortype)|현재 기술의 타입|
| IsForPlayer |bool|플레이어 사용 용도 여부|
| Common/DamagePrecentage | float |공격력 가중치|
| CoolTime | float |기본 쿨타임|
| RotType | [eSkillRotateType](#열거형-eskillrotatetype) |회전 타입|
| CoolTimeRandomRange | float |랜덤 쿨타임 범위(몬스터 전용)|
| InitialCoolTime | float |초기 쿨타임(몬스터 전용)|
| IsExplositionAttack | bool |공격후 몬스터의 사망 여부(자폭형 공격일 경우)|
| TargetTag | [eTargetTag](#열거형-etargettag) |공격 대상 태그|
| UsingCameraActing | bool |카메라 연출 사용 여부|
| CameraActingOnUsingSkill | [CameraActingEventData](#카메라-연출-옵션-cameraactingeventdata-구조체)[] |카메라 연출 옵션 배열 (스킬 사용 즉시 발동)|
| AttackMotion | AnimationClip |공격 모션 애니메이션 클립|
| PreparingDuration | float |준비 동작 모션 시간|
| PreparingTimeAccel | float |준비 동작 모션 가속치|
| AttackDuration | float |공격 동작 모션 시간|
| AttackDurationAccel | float |공격 동작 모션 가속치|
| CancleableDuration | float |후딜레이 시간|
| CancleableDurationAccel | float |후딜레이 가속치|
| ManualSkillEvents | [ManualSkillEvent](#수동-스킬-이벤트-manualskillevent-구조체)[]|시간초 기준으로 발생시킬 스킬 관련 이벤트 데이터|
| UsingAimAssistance | bool |에임 보정 사용 여부|
| AimAisstanceConfig | AimAssistanceConfig |기본 에임 보정 옵션|
| ConosleAimAssistance | AimAssistanceConfig |콘솔플레이 에임 보정 옵션|
| UsingBulletTimeOnSkillInvoked | bool |불릿 타임 발생 여부|
| BulletTimeOnUsingSkill | BulletTimeData |불릿 타임 옵션|
| IsComboAttack | bool |콤보 공격 여부|
| sfIsFirstAttack | bool |첫번째 콤보 공격 여부|
| EnabledComboCount | int |활성화된 콤보 가능 횟수|
| CombableTime | float |공격 종료후 추가 콤보가 가능한 여분의 시간|
| ComboSkillName | string |콤보 공격 이름|
| ComboIndex | int |현재 콤보 공격의 순서|
| SetWaitComboInputManually | bool |콤보 선입력 시간을 지정할지에 관한 여부|
| IsChargingAttack | bool |차징 공격 여부|
| MinChargingTime | float |최소 차징 시간|
| ChargeStep | float[] |차징 단계|
| ChargeConfig | SkillConfigVarient[] |차징 단계별 변경될 SkillConfig들(원본은 1단계 차징으로 사용됨)|
| CounterTrySkill | bool |반격 기능 여부|
| CounterBox | ForceCounterInvokingBox |반격 판정 영역 프리팹|
| CountableType | [eCountableType](#열거형-ecountabletype) |반격 타입 \(쉬운 반격-> 타이밍에 무관하게 항상 성공 , 어려운 반격 -> 타이밍을 맞춰야함)|
| MeleeAttackData | [MeleeAttackData](#근접-공격-옵션-meleeattackdata-구조체)[] |근접 공격 발생 데이터들|
| PointSpecifyAttackData | [PointSpecifyAttackData](#좌표-지정형-공격-옵션-pointspecifyattackdata-구조체)[] |좌표 지정형 공격 발생 데이터들|
| ShootingPointTransName | string |투사체 발사지점 Transform 이름|
| ProjectileData | [ProjectileAttackData](#투사체-공격-옵션-projectileattackdata-구조체)[] |투사체 공격 데이터|
| BuffDataOnUsingSkill | BuffData |버프 발생 옵션|
| BuffDuration | float |버프 지속 시간|
| BuffPower | float |버프 가중치|
| IsChannellingSkill | bool |채널링 공격 여부|
| ChannelingDuration | float |채널링 지속 시간|
| ChannelingAttackData | [MeleeAttackData](#근접-공격-옵션-meleeattackdata-구조체) |채널링 공격 판정 데이터|
| ChannelingProjectileData | [ProjectileAttackData](#투사체-공격-옵션-projectileattackdata-구조체) |채널링 공격 투사체 데이터|
| ChannelingFX | [FXSpawnData](#fx-스폰-옵션-fxspawndata-구조체) |체널링 전용 FX 지속 효과|
| ChannelingLoopSFX | EventReference |채널링 전용 SFX 지속 효과|
| FXSpawnData | [FXSpawnData](#fx-스폰-옵션-fxspawndata-구조체)[] |FX 스폰 데이터들|
| TransitionData | [AttackTransitionData](#스킬-진행중-이동방식-옵션-attacktransitiondata-구조체) |이동 데이터|
| AttackSound | EventReference[] |사운드 발생 FMOD 데이터|

# 카메라 연출 옵션 CameraActingEventData 구조체
| Variable | Type | Description |
|---|---|---|
|ActingData|[eCameraActingType](#열거형-ecameraactingtype-flag)|카메라 연출 동작 종류|
|ShakeType|eShakeType|흔들림 타입|
|ShakeDuration|float|흔들림 지속 시간|
|ShakePower|float|흔들림 정도|
|ShakeFrequency|float|흔들림 주기|
|ShakeCurve|AnimationCurve|흔들림 조정 커브(Power와 Frequency에 가중치로 곱해짐)|
|ZoomInAndOutDuration|float|카메라 줌 지속 시간|
|ZoomInOutCurve|AnimationCurve|줌 조정 커브(현재 카메라 줌 정도에 가중치로 곱해짐)|
|PostProcessingType|[ePostProcessingType](#열거형-epostprocessingtype)|포스트 프로세싱 연출 효과 종류|
|ChromaticAberrationInvokeSetting|[PostProcessingInvokingConfig](#포스트-프로세싱-옵션-postprocessinginvokingconfig-구조체)|ChromaticAberration(색수차 효과) 연출 효과 옵션|
|BrightingInvokeSetting|[PostProcessingInvokingConfig](#포스트-프로세싱-옵션-postprocessinginvokingconfig-구조체)|Brighting (밝기 높힘 효과) 연출 효과 옵션|
|EdgeGrayscaleInvokeSetting|[PostProcessingInvokingConfig](#포스트-프로세싱-옵션-postprocessinginvokingconfig-구조체)|EdgeGrayScale (외각 흑백 처리 효과) 연출 효과 옵션|
|RadialBlurInvokeSetting|[PostProcessingInvokingConfig](#포스트-프로세싱-옵션-postprocessinginvokingconfig-구조체)|RadialBlur(원형 블러링) 연출 효과 옵션|

# 스킬 진행중 이동방식 옵션 AttackTransitionData 구조체
| Variable | Type | Description |
|---|---|---|
| ActionType | [eAttackTransitionType](#열거형-eattacktransitiontype) | 이동 방식 타입 |
| DecellationAmount | float | 감속 정도 (SetDecellationFromCurrentSpeed 타입 사용시 활성화) |
| MoveToSpecificDest | [MoveToSpecificDestData](#지정-목적지-이동-옵션-movetospecificdestdata-구조체)[] | 지정된 목적지 이동 데이터들 (MoveToSpecificDest 타입 사용시 활성화) |
| RushDelay | float | 목표 추적 딜레이 (RushToPlayer 타입 사용시 활성화) |
| RushSpeed | float | 목표 추적 이동 속도 (RushToPlayer 타입 사용시 활성화) |
| TrackRotatingSpeed | float |목표 타게팅 회전 속도 (RushToPlayer 혹은 LookPlayer 타입 사용시 활성화) |
| OffsetAngle | float |목표 타게팅 회전 고정 오프셋 (RushToPlayer 혹은 LookPlayer 타입 사용시 활성화) |
| RotatePerSecond | float |초당 회전 각도 (ConstantRotate 타입 사용시 활성화) |


# 근접 공격 옵션 MeleeAttackData 구조체
| Variable | Type | Description |
|---|---|---|
| AttackCollider | AttackBoxElement |근접 공격 판정 프리팹|
| ColliderRemainTime | float |근접 판정 유지 시간|
| SpawnInShootingPoint | bool |근접 판정 생성 위치를 투사체 생성 위치로 설정|
| Option | [AttackBoxOption](#공격-박스-옵션-attackboxoption-구조체) |근접 공격 판정 옵션 |
| OnHitEffect | [HitEffectData](#피격-연출-옵션-hiteffectdata-구조체) |근접 공격 피격 효과 옵션|
| UseIndicator | bool |범위 표기 FX 사용 여부|
| IndicatorParticle | ParticleBinder |범위 표기용 FX 프리팹|
| IndicatorFXTransformType | [eFXTransformType](#열거형-efxtransformtype) |범위 표기용 FX 위치 타입|

# 투사체 공격 옵션 ProjectileAttackData 구조체
| Variable | Type | Description |
|---|---|---|
| Projectile | ProjectileHandler |투사체 원본 프리팹|
| ShootingType | [eShootingType](#열거형-eshootingtype) |투사체 발사 타입|
| MuzzleFX | [FXSpawnData](#fx-스폰-옵션-fxspawndata-구조체) |투사체 발사 지점 생성 FX|
| ShotgunAmount | int |동시 발사 개수(eShootingType.ShotGun 타입 사용시 활성화)|
| ShotgunAngle | float |동시 발사 투사체 최대 사잇각(eShootingType.ShotGun 타입 사용시 활성화) |
| CircularAmount | int | 원형 발사 투사체 개수(eShootingType.Circular 타입 사용시 활성화)|
| IsMultiShot | bool |연속 발사 여부|
| ShootingAmount | int |연속 발사 횟수 |
| DelayBetweenShooting | float |연속 발사 간격|
| MultiShotAiming | [eMultiShotAimingType](#열거형-emultishootaimingtype) |연속 발사 에임 타입|
| RotateDegreePerShoot | float |각 연속 발사당 회전량 (eMultiShotAimingType.ConstantRotate 사용시 활성화)|
| InitialDegreeOffset | float |초기 회전 각(eMultiShotAimingType.ConstantRotate) |
| OnHitEffect | [HitEffectData](#피격-연출-옵션-hiteffectdata-구조체) |투사체 피격 효과 옵션|
| AttackBoxOption | [AttackBoxOption](#공격-박스-옵션-attackboxoption-구조체) |투사체 공격 판정 오브젝트 설정 옵션|
| ProjectileInfo | ProjectileInfo |투사체 정보 데이타|
| UseIndicator | bool |범위 표기 FX 사용 여부|
| IndicatorParticle | ParticleBinder |범위 표기용 FX 프리팹|
| IndicatorFXTransformType | [eFXTransformType](#열거형-efxtransformtype) |범위 표기용 FX 위치 타입|


# 좌표 지정형 공격 옵션 PointSpecifyAttackData 구조체
| Variable | Type | Description |
|---|---|---|
| AttackCollider | AttackBoxElement |생성될 공격 판정 프리팹|
| ColliderRemainTime | float |공격 판정 지속 시간|
| Option | [AttackBoxOption](#공격-박스-옵션-attackboxoption-구조체) |공격 판정 옵션|
| OnHitEffect | [HitEffectData](#피격-연출-옵션-hiteffectdata-구조체) |피격 효과 옵션|
| SpawnPosition | [ePointSpecifyAttackSpawnType](#열거형-epointspecifyattackspawntype) |생성 위치 타입|
| MaxRange | float |최대 사정거리, 에임 보정이 실패했을때 해당 사정거리 기준으로 생성 (ePointSpecifyAttackSpawnType.TargetAimAssistance 타입 사용시 활성화)|
| SpreadRadius | float |랜덤 원형 범위 (ePointSpecifyAttackSpawnType.SpreadRandomCircle 타입 사용시 활성화)|
| IsMultiSpawn | bool |다중 공격 활성화 여부|
| MultiSpawnCount | int |다중 공격 횟수|
| UseIndicator | bool |공격 범위 표기 여부|
| IndicatingDuration | float |공격 범위 지속 시간|

# 피격 연출 옵션 HitEffectData 구조체
| Variable | Type | Description |
|---|---|---|
| HitffectType | [eHitEffectType](#열거형-ehiteffecttype) |피격 효과 타입|
| KnockbackData | [KnockbackData](#넉백-옵션-knockbackdata-구조체) |넉백 타입(eHitEffectType.Knockback 사용시 활성화)|
| Deacceleration | float | 감속량 (eHitEffectType.Deaccelerate 사용시 활성화)|
| DamagedDuration | float |피격 효과 지속 시간|
| UseCameraActing | bool |피격시 카메라 연출 사용 여부|
| CameraActingOnHit | [CameraActingEventData](#카메라-연출-옵션-cameraactingeventdata-구조체) |피격시 카메라 연출 옵션|
| CameraActInvokingHitCountThreshold | int |카메라 연출 효과를 발생시킬 최소 피격 횟수|
| UseBulletTime | bool |불릿 타임 발생 여부|
| BulletTime | BulletTimeData |불릿 타임 옵션|
| BulletTimeInvokingHitCountThreshold | int |불릿 타임을 발생시킬 최소 피격 횟수|
| UseHitStop | bool |스탑 모션 발생 여부|
| HitStopOption | [HitStopData](#히트-스톱-옵션-hitstopdata-구조체) |스탑 모션 옵션|
| GiveDebuff | bool |버프 부여 여부|
| BuffDataOnHitOrNull | BuffData |디버프 옵션|
| BuffDuration | float |디버프 지속 시간|
| BuffPower1 | float |디버프 가중치 변수 1|
| BuffPower2 | float |디버프 가중치 변수 2|
| BuffPower3 | float |디버프 가중치 변수 3|
| BuffStackPerHit | int |피격당 동시에 쌓이는 버프 스택|
| OnHitFX | ParticleBinder |피격 FX 프리팹|
| OnHitSound | EventReference |피격 사운드|
| DamagedTextColorGradientOrNull | TMP_ColorGradient |피격 데미지 표기 UI 그라데이션 색상|

# 넉백 옵션 KnockbackData 구조체
| Variable | Type | Description |
|---|---|---|
| KnockbackType | [eKnockbackType](#열거형-eknockbacktype) |넉백 타입|
| NockbackPower | float |넉백 정도|
| NockbackTime | float |넉백 시간|
| BilladableCount | int |밀치기 효과 최대 횟수|
| IsInverseDir | bool |넉백 방향을 역방향으로 설정 여부|
| NockBackFX | ParticleBinder |넉백 FX 프리팹|

# 지정 목적지 이동 옵션 MoveToSpecificDestData 구조체
| Variable | Type | Description |
|---|---|---|
| DestDistance | float |이동 거리|
| Duration | float |이동 시간|
| Dir | [eDir](#열거형-edir) |이동 방향(공격 방향 기준 변형 가능)|
| NeedAssistedDestSubtraction | bool |에임 보정 발생시 스킬 사용자 위치 보정치 사용 여부(에임 보정은 몬스터의 위치를 반환함으로 공격의 사거리에 따라 몬스터의 위치보다 얼마만큼 떨어져 있는 곳이 자연스러운지 스킬별로 설정해줘야함)|
| AssistedDestSubtraction | float |에임 보정 위치로 부터 최종 캐릭터 위치 조정값|
| TargetingPlayerIfInRange | bool |몬스터의 플레이어 타게팅 여부|
| PlayerTagettingSubstraction | float |플레이어의 위치로 부터 최종 몬스터 위치 조정값|

# FX 스폰 옵션 FXSpawnData 구조체
| Variable | Type | Description |
|---|---|---|
| FXTransformType | [eFXTransformType](#열거형-efxtransformtype) |FX 위치 설정 타입|
| SpawnInShootingPoint | bool |투사체 발사 지점에서 생성 여부|
| EffectPrefab | GameObject |FX 프리팹|

# 수동 스킬 이벤트 ManualSkillEvent 구조체
| Variable | Type | Description |
|---|---|---|
| EventType | [eSkillEventMarkerType](#열거형-eskilleventmarkertype) |공격 이벤트 타입|
| InvokeTiming | float |공격 이벤트 발생 타이밍|

# 포스트 프로세싱 옵션 PostProcessingInvokingConfig 구조체
| Variable | Type | Description |
|---|---|---|
| SettingType | [eVolumnWeightSettingType](#열거형-evolumnweightsettingtype) |포스트 프로세싱 가중치 설정 타입|
| Duration | float |포스트 프로세싱 효과 지속 시간|
| PowerCurve | AnimationCurve |포스트 프로세싱 가중치 커브(매 프레임 Duration 시간 동안 타겟 포스트 프로세싱의 가중치를 설정해줌)|
| Power | float |포스트 프로세싱 가중치 값 (포스트 프로세싱의 가중치를 커브와 상관없이 고정값으로 설정해줌)|

# 히트 스톱 옵션 HitStopData 구조체
| Variable | Type | Description |
|---|---|---|
| SlowDownAmount | float |히트 스탑 감속량|
| SlowTime | float |히트 스탑 지속 시간|

# 공격 박스 옵션 AttackBoxOption 구조체
| Variable | Type | Description |
|---|---|---|
| AttackBoxHitType | [eAttackBoxType](#열거형-eattackboxtype) |공격 박스 타입 옵션|
| MaxHitCounts | int |최대 다단히트 가능 횟수 (eAttackBoxType.MultiHit 사용시 활성화)|
| HitInterval | float |다단히트 간격 (eAttackBoxType.MultiHit 사용시 활성화)|
| InvokeDamagedActorPerHit | bool |다단히트시 피격 효과를 각 피격마다 발생 시키는지에 관한 여부 (eAttackBoxType.MultiHit 사용시 활성화)|
| DivideFinalDamageByMaxHitCount | bool |다단히트시 최종 데미지를 최대 피격 횟수로 나눠줄지에 관한 여부 |
| SpawnType | [eAttackboxSpawnType](#열거형-eattackboxspawntype) |공격 박스 스폰 위치 타입 옵션|
| LifetimeType | [eAttackBoxLifetimeType](#열거형-eattackboxlifetimetype) |공격 박스 생명 주기 타입 옵션|
| RemainDuration | float |공격 박스 지속 시간|
| IsMeleeboxProjectile | bool |공격 박스 이동 여부|
| Speed | float |공격 박스 이동 속도|
| IsThrowingMeleebox | bool |투척형 공격 여부|
| ThrowingDuration | float |투척 속도|

# 열거형 eShootingType
| Value | Description |
|---|---|
| OneShot |공격 방향 기준 단발 사격|
| ShotGun |공격 방향 기준 산탄 사격|
| Circular |원형 사격|

# 열거형 eMultiShotAimingType
| Value | Description |
|---|---|
| NoneChangeOnFirstShoot |초기 발사 방향으로 부터 변화 없음|
| TrackingPerShoot |매 발사 마다 적을 추적함|
| ConstantRotate |매 발사 마다 고정 각도를 회전함|

# 열거형 eDir
| Value | Description |
|---|---|
| Front |공격 방향 유지|
| Left |공격 방향 기준 수직한 왼쪽 방향으로 설정|
| Right |공격 방향 기준 수직한 오른쪽 방향으로 설정|

# 열거형 eVolumnWeightSettingType
| Value | Description |
|---|---|
| Constant |포스트 프로세싱 가중치를 고정으로 설정|
| CustomCurve |포스트 프로세싱 가중치를 커브를 통해 설정|

# 열거형 eCameraActingType (Flag)
| Value | Description |
|---|---|
| None |카메라 엑팅 없음|
| Shake |흔들림 연출 사용|
| ZoomInAndOutCurve |줌 인 줌 아웃 연출 사용|

# 열거형 ePostProcessingType
| Value | Description |
|---|---|
| None | |
| ChromaticAberration |색수차 효과 |
| EdgeGrayscale |외각 회색음영 효과|
| Bright |밝기 효과|
| RadialBlur |원형 블러 효과|
| FullGrayScale |전체 화면 회색음영 효과|

# 열거형 eAttackTransitionType
| Value | Description |
|---|---|
| None |공격 이동 없음|
| SetDecellationFromCurrentSpeed |현재 속도를 기준으로 감속|
| MoveToSpecificDest |지정된 위치로 이동|
| Pause |완전 정지|
| RushToPlayer |플레이어를 향해 이동 및 회전|
| LookPlayer |플레이어를 향해 회전|
| ConstantRotate |고정값 회전|

# 열거형 eSkillRangeType
| Value | Description |
|---|---|
| None |공격 범위 없음|
| Melee |근접 공격|
| Rangend |원거리 공격|

# 열거형 eKnockbackType
| Value | Description |
|---|---|
| PushToAttackDir |공격 방향으로 넉백|
| CircularToHitPoint |공격 판정 기준 원형으로 넉백|

# 열거형 eHitEffectType
| Value | Description |
|---|---|
| None |피격 효과 없음|
| Knockback |넉백 효과 발생|
| Stunned |스턴 효과 발생|
| Deaccelerate |감속 효과 발생|
| Pause |정지 효과 발생|

# 열거형 eSkillProgressState
| Value | Description |
|---|---|
| None |스킬 진행 타입|
| Preparing |준비 동작|
| OnAttack |공격중 동작|
| Cancelable |후딜레이 동작|

# 열거형 eTargetTag
| Value | Description |
|---|---|
| Player |플레이어를 공격 대상으로 설정|
| Enemey |몬스터를 공격 대상으로 설정|
| All |모든 대상을 공격 대상으로 설정|
| PlayerAndObject |플레이어와 피격 가능 오브젝트로 설정 |플레이어를 공격 대상으로 설정|
| Enemey |적군을 공격 대상으로 설정|
| All |모든 대상을 공격 대상으로 설정|
| PlayerAndObject |플레이어와 피격 가능 오브젝트로 설정|

# 열거형 eStunType
| Value | Description |
|---|---|
| DefaultStun |기본 스턴 타입 |

# 열거형 eCountableType
| Value | Description |
|---|---|
| NoneCountable |카운터 불가능|
| EasyCountable |쉬운 카운터(근처 공격하는 적을 탐지하여 자동으로 패링 시켜줌!) |
| HardCountable |어려운 카운터(정확한 피격 타이밍에 반격을 발생시켜야 패링 판정이 활성화)|

# 열거형 eSkillRotateType
| Value | Description |
|---|---|
| Default |현재 캐릭터의 정면 방향을 공격방향으로 부드럽게 회전|
| Immediate |현재 캐릭터의 정면 방향을 공격 방향으로 즉시 회전 |
| Fix |현재 캐릭터의 정면 방향을 공격 방향과 상관없이 유지|

# 열거형 eAttackBoxType
| Value | Description |
|---|---|
| OneHitOnly |단일 타격 설정|
| MultiHit |다단 히트 활성화|

# 열거형 eAttackboxSpawnType
| Value | Description |
|---|---|
| Default |캐릭터 Transform 하위에 생성|
| SpawnInProjectile |투사체 발사 지점을 기준으로 생성|
| SpawnInGlobalAttackBox |캐릭터 위치를 기준으로 생성, 하이어라키상 최상단에 생성|

# 열거형 ePointSpecifyAttackSpawnType
| Value | Description |
|---|---|
| TargetAimAssistance |에임 보정 위치에 좌표를 지정|
| SpreadRandomCircle |원형 범위 안 랜덤 위치로 좌표를 지정|
| TargetPlayer |플레이어 위치를 고정으로 좌표를 지정|

# 열거형 eFXTransformType
| Value | Description |
|---|---|
| World |캐릭터 위치를 기준으로 생성, 생성후 위치를 변경하지 않음|
| Local |캐릭터 위치를 기준으로 생성, 생성후 캐릭터의 위치와 회전을 따라감|
| PositionOnlyWorld |캐릭터 위치를 기준으로 생성, 생성후 캐릭터의 위치를 따라감|
| RotationOnlyWorld |캐릭터의 위치를 기준으로 생성, 생성후 캐릭터의 회전값을 따라감|
| LocalActivatedCharacter |캐릭터 위치를 기준으로 생성, 생성후 플레이어를 따라감(캐릭터를 변경 하더라도!)|

# 열거형 eSkillEventMarkerType
| Value | Description |
|---|---|
| InvokeSkillFX |FX 생성|
| InvokeAttackBox |공격 판정 생성|
| InvokeProjectile |투사체 생성|
| ChangeProgressState |공격 상태 변경(준비동작 -> 공격 중 -> 후딜레이)|
| InvokeSound |사운드 발생|
| InvokeTransition |이동 시작|
| SetComboInputWait |추가 콤보 입력 가능 시점 설정|
| InvokeCameraActing |카메라 연출 효과 발생|
| PowerOverwalrming |무적 설정 토글(비활성화 <-> 활성화)|
| InvokePointSpecifyAttack |좌표 지정 공격 생성|
| ChanellingAttack |채널링 어택 시작 시점 설정|
| Count |이벤트 타입 종류 갯수|

# 열거형 eActorType
| Value | Description |
|---|---|
| None |없음|
| Dash |대시 엑팅|
| NormalAttack |일반 공격 엑팅|
| NormalChargeAttack |일반 차징 공격 엑팅|
| SpecialAttack |특수 공격 엑팅|
| DashAttack |대시 공격 엑팅|
| SwitchAttack |스왑 공격 엑팅|
| Damaged |피격 엑팅|
| AIMovement |AI 이동 엑팅|
| AIAttack |AI 공격 엑팅|
| InputMovement |입력 이동 엑팅|
| Appearance |스폰 엑팅|
| Dead |사망 엑팅|
| UltimateAttack |궁극기 공격 엑팅|
| CounterAttack |카운터 공격 엑팅|
| TryCounterAttack |카운터 시도 공격 엑팅|
| Count |엑팅 타입 갯수|

# 열거형 eAttackBoxLifetimeType
| Value | Description |
|---|---|
| ClearWithSkillActor |진행중인 스킬 종료시 즉시 비활성화|
| HandleWithManual |진행중인 스킬 종료 여부와 상관없이 유지|
| HandleByAnotherComponent |다른 컴포넌트에 의하여 관리됨|
| DeactiveWithParticleBinder |FX와 싱크를 맞춰 비활성화 됨|
