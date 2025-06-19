public class PlayerAttackState : StateMachineBase
{
    public PlayerAttackState(string _animName) : base(_animName) { }
    public override void EntryState(ActorBase actor)
    {
        actor.animator.SetTrigger("attackTrigger");
    }

    public override void ExitState(ActorBase actor)
    {

    }

    public override void PhyiscsState(ActorBase actor)
    {

    }

    public override void UpdateState(ActorBase actor)
    {
        actor.Attack();

        if (!actor.isRunning)
            StateMachineBase.ChangeCurrentState(actor, actor.idleState);
        if (actor.isRunning)
            StateMachineBase.ChangeCurrentState(actor, actor.moveState);
    }
}
