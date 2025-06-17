using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    public IState curState;
    public Dictionary<StateType, IState> states;
    public BlackBoard blackboard;
    public FSM(BlackBoard blackboard)
    {
        states = new Dictionary<StateType, IState>();
        this.blackboard = blackboard;
    }
    public void AddState(StateType stateType,IState state)
    {
        if (states.ContainsKey(stateType))
        {
            Debug.LogError("?");
            return;
        }
        states.Add(stateType, state);
    }
    public void SwitchState(StateType stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            Debug.LogError("?");
            return;
        }
        if (curState != null)
        {
            curState.OnExit();
        }
        curState = states[stateType];
        curState.OnEnter();
    }
    public void OnUpdate()
    {
        curState.OnUpdate();
    }
}
[Serializable]
public class BlackBoard
{

}