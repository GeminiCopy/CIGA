using UnityEngine;

public abstract class ActorBase : MonoBehaviour
{
    public float currentHealth { get; set; }         //当前健康值
    public float maxHealth { get; protected set; }   //最大生命
    public float attackValue { get; protected set; } //攻击力
    public float powerValue { get; protected set; }  //体力
    public float moveSpeed { get; protected set; }
    public bool isRunning { get; set; }
    public bool isAttack { get; set; }
    public bool isHurt { get; set; }
    public bool isDead { get; set; }
    public int idleIndex { get; set; }
    public int attackIndex { get; set; }
    public Rigidbody rigid { get; protected set; }
    public Animator animator { get; protected set; }
    public StateMachineBase currentState { get; set; }
    public StateMachineBase moveState { get; protected set; }
    public StateMachineBase idleState { get; protected set; }
    public StateMachineBase hurtState { get; protected set; }
    public StateMachineBase deadState { get; protected set; }
    public StateMachineBase attackState { get; protected set; }
    protected abstract void Awake();
    protected abstract void Start();
    protected abstract void Update();
    protected abstract void FixedUpdate();
    public abstract void Movement();
    public abstract void Attack();
    public abstract void TakeDamage();
    public abstract void FaceDirect();
}
