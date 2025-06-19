public abstract class StateMachineBase                  //状态机基类
{
    public StateMachineBase(string _animName) { animName = _animName; }
    public string animName { get; protected set; }
    public abstract void EntryState(ActorBase actor);   //进入状态
    public abstract void UpdateState(ActorBase actor);  //更新状态
    public abstract void PhyiscsState(ActorBase actor); //物理状态
    public abstract void ExitState(ActorBase actor);    //退出状态
    public static void ChangeCurrentState(ActorBase actor, StateMachineBase newState)
    {
        actor.currentState?.ExitState(actor);  //如果actor为空则直接改变状态而不需要退出当前状态
        actor.currentState = newState;
        actor.currentState.EntryState(actor);
    }
}