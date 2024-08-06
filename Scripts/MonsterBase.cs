using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MonsterStateMachine;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterBase : CharacterBase
{
    #region ������
    public List<string> dropList = new List<string>(); // �� ���Ͱ� ����ϰ� �Ǵ� ������ ����Ʈ(�̸����� �޾ư�����)

    [HideInInspector]public NavMeshAgent agent;  // �׺�Ž�������Ʈ
    public GameObject player { get; private set; }  // �÷��̾�

    [Header("AI requirements")]
    public float maxAreaDistance;  // ������ ��������(���� ������)
    public float viewAngle;        // ������ �þ߰�
    public float viewDistance;     // ������ �þ߰Ÿ�
    public float attackDistance;  // ���Ͱ� ���� ������ �Ÿ�
    public int randomAttackNum; // ������ ���� ����

    public MonsterHP hpBar; // ü�¹�

    public Vector3 originPosition { get; protected set; } // ������ ù ��ġ�� �������� �� ���̹Ƿ� ���� ��ġ�� �����ѳ���

    public bool isDead { get; protected set; }             // ���Ͱ� ���� �����ΰ�(��Ȱ��ȭ)

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

        currentState = States[(int)MonsterState.Patrol]; //  ���� �������·�

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
