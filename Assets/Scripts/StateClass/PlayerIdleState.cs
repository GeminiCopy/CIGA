using UnityEngine;
public class PlayerIdleState : StateMachineBase
{
    public PlayerIdleState(string _animName) : base(_animName) { }
    public override void EntryState(ActorBase actor)
    {
        
    }

    public override void ExitState(ActorBase actor)
    {

    }

    public override void PhyiscsState(ActorBase actor)
    {

    }

    public override void UpdateState(ActorBase actor)
    {
        if (actor.isRunning)
            StateMachineBase.ChangeCurrentState(actor, actor.moveState);
        if (actor.isAttack)
            StateMachineBase.ChangeCurrentState(actor, actor.attackState);

        actor.animator.SetInteger("idleIndex", actor.idleIndex);
    }
}
