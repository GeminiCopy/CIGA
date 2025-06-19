using UnityEngine;
public class PlayerMoveState : StateMachineBase
{
    public PlayerMoveState(string _animName) : base(_animName) { }
    public override void EntryState(ActorBase actor)
    {
        actor.animator.SetBool(animName, true);
    }

    public override void ExitState(ActorBase actor)
    {
        actor.animator.SetBool(animName, false);
    }

    public override void PhyiscsState(ActorBase actor)
    {
        
    }

    public override void UpdateState(ActorBase actor)
    {
        Debug.Log("move state");
        if (!actor.isRunning)
            StateMachineBase.ChangeCurrentState(actor, actor.idleState);
        if (actor.isAttack)
            StateMachineBase.ChangeCurrentState(actor, actor.attackState);

        actor.Movement();
    }
}
