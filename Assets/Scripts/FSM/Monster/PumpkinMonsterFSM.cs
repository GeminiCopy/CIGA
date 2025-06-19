using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class PumpkinMonsterFSM : MonoBehaviour
{
    private FSM fsm;
    public PumpkinMonsterBlackBoard blackboard;
    public bool drawGizmos;
    void Start()
    {
        fsm = new FSM(blackboard);
        fsm.AddState(StateType.Enter, new PumpkinMonsterEnterState(fsm));
        fsm.AddState(StateType.Idle, new PumpkinMonsterIdleState(fsm));
        fsm.AddState(StateType.ChaseTarget, new PumpkinMonsterChaseTargetState(fsm));
        fsm.AddState(StateType.Attack, new PumpkinMonsterAttackState(fsm));
        fsm.AddState(StateType.KeepDistance, new PumpkinMonsterKeepDistanceState(fsm));
        fsm.AddState(StateType.Dead, new PumpkinMonsterDeadState(fsm));
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
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(gameObject.transform.position, blackboard.keepDistance);
        }
    }
    private class PumpkinMonsterEnterState : IState
    {
        private FSM fsm;
        private PumpkinMonsterBlackBoard blackboard;

        private float enterTime;
        public PumpkinMonsterEnterState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as PumpkinMonsterBlackBoard;
        }
        public void OnEnter()
        {
            enterTime = 0.0f;
            Debug.Log("进入");
            var target = GameObject.FindWithTag("Player");
            if (target == null)
            {
                Debug.Log("CANT FIND");
                Destroy(blackboard.self);
            }
            else
            {
                blackboard.target = target;
            }
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
    private class PumpkinMonsterIdleState : IState
    {
        private FSM fsm;
        private PumpkinMonsterBlackBoard blackboard;
        public PumpkinMonsterIdleState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as PumpkinMonsterBlackBoard;
        }
        public void OnEnter()
        {
            Debug.Log("IDLE");

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
            else if(Vector3.Distance(blackboard.target.transform.position, blackboard.self.transform.position) < blackboard.keepDistance && blackboard.attackCD > 0)
            {
                fsm.SwitchState(StateType.KeepDistance);
            }
        }
    }
    private class PumpkinMonsterChaseTargetState : IState
    {
        private FSM fsm;
        private PumpkinMonsterBlackBoard blackboard;
        public PumpkinMonsterChaseTargetState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as PumpkinMonsterBlackBoard;
        }
        public void OnEnter()
        {
            Debug.Log("追逐目标" + blackboard.target.transform.position);
            blackboard.aniamtor.SetBool("IsMove", true);
            blackboard.navmeshAgent.destination = blackboard.target.transform.position;
            blackboard.navmeshAgent.isStopped = false;
        }

        public void OnExit()
        {
            blackboard.aniamtor.SetBool("IsMove", false);
            blackboard.navmeshAgent.velocity = Vector3.zero;
            blackboard.navmeshAgent.isStopped = true;
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
    private class PumpkinMonsterKeepDistanceState : IState
    {
        private FSM fsm;
        private PumpkinMonsterBlackBoard blackboard;
        private float findPointCD;
        private float maxFindPointCD;
        public PumpkinMonsterKeepDistanceState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as PumpkinMonsterBlackBoard;
        }
        public void OnEnter()
        {
            Debug.Log("逃离");
            maxFindPointCD = 2f;
            findPointCD = -1;
            blackboard.navmeshAgent.speed = blackboard.goBackSpeed;
            blackboard.aniamtor.SetBool("IsMove", true);
            blackboard.navmeshAgent.isStopped = false;
            blackboard.navmeshAgent.updateRotation = false;
        }

        public void OnExit()
        {
            blackboard.navmeshAgent.speed = blackboard.runSpeed;
            blackboard.aniamtor.SetBool("IsMove", false);
            blackboard.navmeshAgent.velocity = Vector3.zero;
            blackboard.navmeshAgent.isStopped = true;
            blackboard.navmeshAgent.updateRotation = true;
        }

        public void OnUpdate()
        {
            blackboard.attackCD -= Time.deltaTime;
            findPointCD -= Time.deltaTime;
            //朝向玩家应该做一个平滑计算
            blackboard.self.transform.LookAt(blackboard.target.transform.position);
            if (Vector3.Distance(blackboard.target.transform.position, blackboard.self.transform.position) > blackboard.checkEnemyDistance)
            {
                fsm.SwitchState(StateType.Idle);
            }
            else if(blackboard.attackCD < 0)
            {
                fsm.SwitchState(StateType.ChaseTarget);
            }
            else/* if(findPointCD <= 0)*/
            {
                findPointCD = maxFindPointCD;
                Vector3 dir = (blackboard.self.gameObject.transform.position - blackboard.target.gameObject.transform.position).normalized;
                Vector3 targetPos = blackboard.self.transform.position + 
                    dir * (blackboard.keepDistance - Vector3.Distance(blackboard.self.gameObject.transform.position, blackboard.target.gameObject.transform.position));
                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetPos, out hit, blackboard.searchRange, NavMesh.AllAreas))
                {
                    blackboard.navmeshAgent.SetDestination(hit.position);
                    blackboard.aniamtor.SetBool("IsMove", true);
                    Debug.Log("找到逃离点" + hit.position);
                }
                else
                {
                    Debug.Log("未找到可移动点");
                }
            }
            if(Vector3.Distance(blackboard.self.transform.position, blackboard.navmeshAgent.destination) < 0.5f)
            {
                blackboard.aniamtor.SetBool("IsMove", false);
            }
        }
    }
    private class PumpkinMonsterAttackState : IState
    {
        private FSM fsm;
        private PumpkinMonsterBlackBoard blackboard;

        private float time1;
        public PumpkinMonsterAttackState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as PumpkinMonsterBlackBoard;
        }
        public void OnEnter()
        {
            Debug.Log("攻击");
            time1 = 2.6f;//blackboard.doubleAttackCD;
            //doubleAttack = false;
            blackboard.aniamtor.SetTrigger("IsAttack");
            //攻击碰撞判断
        }

        public void OnExit()
        {
            blackboard.attackCD = blackboard.maxAttackCD;
        }

        public void OnUpdate()
        {
            time1 -= Time.deltaTime;
            if (time1 <= 0)
            {
                //fsm.SwitchState(StateType.KeepDistance);
                fsm.SwitchState(StateType.KeepDistance);
            }
        }
    }
    private class PumpkinMonsterDeadState : IState
    {
        private FSM fsm;
        private PumpkinMonsterBlackBoard blackboard;

        private float enterTime;
        public PumpkinMonsterDeadState(FSM fsm)
        {
            this.fsm = fsm;
            this.blackboard = fsm.blackboard as PumpkinMonsterBlackBoard;
        }
        public void OnEnter()
        {
            blackboard.aniamtor.SetBool("IsDead", true);
            enterTime = 3.0f;
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {
            enterTime -= Time.deltaTime;
            if (enterTime <= 0)
            {
                //动画播完，进行处理，暂时销毁
                GameObject.Destroy(blackboard.self);
            }
        }
    }
}
[Serializable]
public class PumpkinMonsterBlackBoard : BlackBoard
{
    public Animator aniamtor;
    public NavMeshAgent navmeshAgent;
    //public Rigidbody rigidbody;
    public float checkEnemyDistance;
    public float attackRange;
    public float keepDistance;
    public float searchRange;
    public float runFromTargetDistance;
    public float attackCD;
    public float maxAttackCD;
    public float goBackSpeed;
    public float runSpeed;
    //public float bombRange;
    public GameObject target;
    public GameObject self;
}
