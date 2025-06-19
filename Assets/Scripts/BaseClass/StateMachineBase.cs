public abstract class StateMachineBase                  //״̬������
{
    public StateMachineBase(string _animName) { animName = _animName; }
    public string animName { get; protected set; }
    public abstract void EntryState(ActorBase actor);   //����״̬
    public abstract void UpdateState(ActorBase actor);  //����״̬
    public abstract void PhyiscsState(ActorBase actor); //����״̬
    public abstract void ExitState(ActorBase actor);    //�˳�״̬
    public static void ChangeCurrentState(ActorBase actor, StateMachineBase newState)
    {
        actor.currentState?.ExitState(actor);  //���actorΪ����ֱ�Ӹı�״̬������Ҫ�˳���ǰ״̬
        actor.currentState = newState;
        actor.currentState.EntryState(actor);
    }
}