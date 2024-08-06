using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MonsterStateMachine;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterBase : CharacterBase
{
    #region 성가셔
    public List<string> dropList = new List<string>(); // 이 몬스터가 드롭하게 되는 아이템 리스트(이름으로 받아가도록)

    [HideInInspector]public NavMeshAgent agent;  // 네비매쉬에이전트
    public GameObject player { get; private set; }  // 플레이어

    [Header("AI requirements")]
    public float maxAreaDistance;  // 몬스터의 영역범위(원의 반지름)
    public float viewAngle;        // 몬스터의 시야각
    public float viewDistance;     // 몬스터의 시야거리
    public float attackDistance;  // 몬스터가 공격 가능한 거리
    public int randomAttackNum; // 랜덤한 공격 개수

    public MonsterHP hpBar; // 체력바

    public Vector3 originPosition { get; protected set; } // 몬스터의 첫 우치는 고정으로 할 것이므로 최초 위치를 기억시켜놓음

    public bool isDead { get; protected set; }             // 몬스터가 죽은 상태인가(비활성화)

    #endregion

    public MonsterStateBase[] States = new MonsterStateBase[(int)MonsterState.Length];
    private MonsterStateBase currentState;
    public bool isAttacking;

    protected override void Start()
    {
        base.Start();
        player = GameManager.player.gameObject;

        States[(int)MonsterState.Patrol] = new MonsterStateMachine.PatrolState();
        States[(int)MonsterState.Chase] = new MonsterStateMachine.ChaseState();
        States[(int)MonsterState.Combat] = new MonsterStateMachine.CombatState();

        currentState = States[(int)MonsterState.Patrol]; //  최초 순찰상태로

        originPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
       // UI_Manager.Instance.MonsterHpBarSet(this);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void MovementUpdate()
    {
        currentState.Execute(this);
    }

    protected override void AnimationUpdate()
    {
        anim?.SetFloat("Velocity", agent.velocity.magnitude);
    }

    public void ChangeState(MonsterStateBase wantState)
    {
        currentState.Exit(this);
        currentState = wantState;
        currentState.Enter(this);
    }

    public override void Dead()
    {
        anim?.SetTrigger("Die");
    }

    public override void MoveTo(Vector3 wantValue) { }
    public override void Attack() { }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Weapon")
        {


            ApplyDamage(GameManager.player.stat.AttackDamage, GameManager.player);
        }
    }

}
