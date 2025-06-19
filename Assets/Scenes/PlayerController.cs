using UnityEngine;
using System.Collections;
public class PlayerController : ActorBase
{
    float horizontalInput;

    float verticalInput;
    protected override void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        if (null == rigid) Debug.LogError("Get rigidbody component failed!");

        animator = GetComponentInChildren<Animator>();
        if (null == animator) Debug.LogError("Get animator component failed!");
    }
    protected override void Start()
    {
        moveSpeed = 8;
        currentHealth = 1;
        maxHealth = 100;
        attackValue = 20;

        idleState = new PlayerIdleState("isIdle");
        moveState = new PlayerMoveState("isRunning");
        deadState = null;
        attackState = new PlayerAttackState("isAttack");
        StateMachineBase.ChangeCurrentState(this, idleState);  //初始化玩家状态为idle
        StartCoroutine(IdleIndexInc());
    }
    protected override void Update()
    {
        CheckPlayerInput();  //检测玩家输入
        
        CheckStateFlag();

        FaceDirect();
        
        currentState.UpdateState(this);
    }
    protected override void FixedUpdate()
    {
        currentState.PhyiscsState(this);
    }
    public override void Attack()
    {
        rigid.velocity = Vector3.zero;  //攻击时不能移动
        Debug.Log("Attack");
    }

    public override void FaceDirect()
    {
        Vector3 rotate = new Vector3(0,horizontalInput + verticalInput ,0).normalized;
        transform.localRotation = Quaternion.Euler(rotate);
    }

    public override void Movement()
    {
        rigid.velocity = new Vector3(horizontalInput * moveSpeed, 0, verticalInput * moveSpeed);
    }

    public override void TakeDamage()
    {

    }

    public void CheckPlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isAttack = Input.GetKeyDown(KeyCode.J);
    }

    public void CheckStateFlag()
    {
        isRunning = horizontalInput != 0 || verticalInput != 0;
    }

    IEnumerator IdleIndexInc()
    {
        while(true)
        {
            //idleIndex = (idleIndex + 1) % 3;
            yield return new WaitForSeconds(15f);
            idleIndex = Random.Range(1, 3);
            Debug.Log(idleIndex);
        }
    }
}