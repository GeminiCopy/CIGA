using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class HandMonsterFSM : MonoBehaviour
{
    private FSM fsm;
    public HandMonsterBlackBoard blackboard;
    public bool drawGizmos;
    void Start()
    {
        fsm = new FSM(blackboard);
        fsm.AddState(StateType.Enter, new HandMonsterEnterState(fsm));
        fsm.AddState(StateType.Idle, new HandMonsterIdleState(fsm));
        fsm.AddState(StateType.ChaseTarget, new HandMonsterChaseTargetState(fsm));
        fsm.AddState(StateType.Attack, new HandMonsterAttackState(fsm));
        fsm.AddState(StateType.Dead, new HandMonsterDeadState(fsm));
        fsm.SwitchState(StateType.Enter);
    }
    void Update()
    {
        fsm.OnUpdate();
    }
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gameObject.transform.position, blackboard.checkEnemyDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gameObject.transform.position, blackboard.attackRange);
        }
    }
    
    private class HandMonsterEnterState : IState
    {
        private FSM fsm;
        private HandMonsterBlackBoard blackboard;

        private float enterTime;
        public HandMonsterEnterState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as HandMonsterBlackBoard;
        }
        public void OnEnter()
        {
            enterTime = 1.3f;
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {
            enterTime -= Time.deltaTime;
            if (enterTime <= 0)
            {
                fsm.SwitchState(StateType.Idle);
            }
        }
    }
    private class HandMonsterIdleState : IState
    {
        private FSM fsm;
        private HandMonsterBlackBoard blackboard;
        public HandMonsterIdleState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as HandMonsterBlackBoard;
        }
        public void OnEnter()
        {

        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {
            blackboard.attackCD -= Time.deltaTime;
            if (Vector3.Distance(blackboard.target.transform.position, blackboard.self.transform.position) < blackboard.checkEnemyDistance && blackboard.attackCD <= 0)
            {
                fsm.SwitchState(StateType.ChaseTarget);
            }
        }
    }
    private class HandMonsterChaseTargetState : IState
    {
        private FSM fsm;
        private HandMonsterBlackBoard blackboard;
        public HandMonsterChaseTargetState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as HandMonsterBlackBoard;
        }
        public void OnEnter()
        {
            blackboard.aniamtor.SetBool("IsMove", true);
            blackboard.navmeshAgent.destination = blackboard.target.transform.position;
            blackboard.navmeshAgent.isStopped = false;
        }

        public void OnExit()
        {
            blackboard.aniamtor.SetBool("IsMove", false);
            blackboard.navmeshAgent.velocity = Vector3.zero;
            blackboard.navmeshAgent.isStopped = true;
            //blackboard.rigidbody.velocity = Vector3.zero;
        }

        public void OnUpdate()
        {
            blackboard.navmeshAgent.destination = blackboard.target.transform.position;
            if (Vector3.Distance(blackboard.target.transform.position, blackboard.self.transform.position) < blackboard.attackRange &&
               Vector3.Angle(blackboard.self.gameObject.transform.forward, blackboard.target.gameObject.transform.position - blackboard.self.gameObject.transform.position) < 30)
            {
                fsm.SwitchState(StateType.Attack);
            }
            else if (Vector3.Distance(blackboard.target.transform.position, blackboard.self.transform.position) > blackboard.checkEnemyDistance)
            {
                fsm.SwitchState(StateType.Idle);
            }
        }
    }
    private class HandMonsterAttackState : IState
    {
        private FSM fsm;
        private HandMonsterBlackBoard blackboard;

        private float time1;
        //private bool doubleAttack;
        public HandMonsterAttackState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as HandMonsterBlackBoard;
        }
        public void OnEnter()
        {
            time1 = 1.6f;//blackboard.doubleAttackCD;
            //doubleAttack = false;
            blackboard.aniamtor.SetTrigger("IsAttack");
            //¹¥»÷Åö×²ÅÐ¶Ï
        }

        public void OnExit()
        {
            blackboard.attackCD = blackboard.maxAttackCD;
        }

        public void OnUpdate()
        {
            time1 -= Time.deltaTime;
            //if(time1 <= 0 && !doubleAttack)
            //{
            //    doubleAttack = true;
            //    time1 = 1.6f;
            //    blackboard.aniamtor.SetTrigger("IsAttack");
            //    //¹¥»÷Åö×²ÅÐ¶Ï
            //}
            //if (time1 <= 0 && doubleAttack)
            //{
            //    fsm.SwitchState(StateType.Idle);
            //}
            if (time1 <= 0)
            {
                fsm.SwitchState(StateType.Idle);
            }
        }
    }
    private class HandMonsterDeadState : IState
    {
        private FSM fsm;
        private HandMonsterBlackBoard blackboard;

        private float enterTime;
        public HandMonsterDeadState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as HandMonsterBlackBoard;
        }
        public void OnEnter()
        {
            blackboard.aniamtor.SetBool("IsDead", true);
            enterTime = 2.6f;
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {
            enterTime -= Time.deltaTime;
            if (enterTime <= 0)
            {
                //¶¯»­²¥Íê£¬½øÐÐ´¦Àí£¬ÔÝÊ±Ïú»Ù
                GameObject.Destroy(blackboard.self);
            }
        }
    }
}
[Serializable]
public class HandMonsterBlackBoard : BlackBoard
{
    public Animator aniamtor;
    public NavMeshAgent navmeshAgent;
    //public Rigidbody rigidbody;
    public float checkEnemyDistance;
    public float attackRange;
    public float doubleAttackCD;
    public float attackCD;
    public float maxAttackCD;
    public GameObject target;
    public GameObject self;
}