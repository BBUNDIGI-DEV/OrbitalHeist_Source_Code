using UnityEngine;

public class InputMovementActor : ActorBase
{
    private readonly MovementConfig CONFIG;
    private readonly CharacterStatus PLAYER_STATUS;

    public InputMovementActor(ActorStateMachine ownerStateMachine, MovementConfig config, CharacterStatus status)
        : base(ownerStateMachine, config.BaseConfig, config.name)
    {
        Debug.Assert(ownerStateMachine.IsPlayerActor, "InputMovementActor is for player");
        CONFIG = config;
        PLAYER_STATUS = status;
    }

    public override void UpdateActing(float deltaTime)
    {
        float speed = CONFIG.Speed + PLAYER_STATUS.ExtraSpeed + PlayerManager.Instance.GlobalPlayerStatus.GlobalSpeed;
        Vector3 moveDir = InputManager.Instance.MoveDir;
        Vector3 cameraForward = CameraManager.Instance.MainCamera.transform.forward;
        cameraForward.y = 0.0f;
        cameraForward.Normalize();
        moveDir =  Quaternion.LookRotation(cameraForward) * moveDir;
        Vector3 newVelocity = moveDir * speed;
        newVelocity = new Vector3(newVelocity.x, mRB.velocity.y, newVelocity.z);
        mRB.EnrollSetVelocity(newVelocity, eActorType.InputMovement);

        if (moveDir != Vector3.zero)
        {
            Vector3 newDirection = Vector3.RotateTowards(mRB.GetForward(), moveDir, CONFIG.RotSpeed * Time.deltaTime, 0.0f);
            mRB.EnrollLookRotation(newDirection, ActorType);
        }

        Anim.UpdateMovementAnim(mRB.velocity);
    }

    public override void StopActing()
    {

    }

    public override void DestoryActor()
    {
    }
}
