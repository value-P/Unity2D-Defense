using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateMachine;

[RequireComponent(typeof(PlayerStatus))]
public class PlayerBase : MonoBehaviour
{
    public static InteractBase interactFocus; // 내가 보고있는 상호작용 대상
    public static bool mouseFix // 마우스 고정

    {
        get
        {
            return Cursor.lockState == CursorLockMode.Locked; // 마우스가 잠겼는지 안잠겼는지
        }

        set
        {
            if (value) // 잠금(마우스 숨김)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else // 잠금해제(마우스 보임)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public GameObject appearance;

    public PlayerStateBase[] States = new PlayerStateBase[(int)PlayerState.Length]; // 캐릭터의 상태머신들 배열
    public PlayerStateBase currentState; // 현재 캐릭터의 상태

    public Animator     anim { get; private set; }
    public PlayerStatus stat { get; private set; }
    public Rigidbody    rigid { get; private set; }

    /// <summary> 이 캐릭터가 지니고있는 카메라회전체 </summary>
    public Transform cameraArm; 

    Transform lastHit;      // 마지막으로 닿은 녀석을 챙긴다

    [SerializeField] float verticalAngle = 15; // 카메라Arm 수직각도(수 올리면 카메라 위로, 시야는 아래로)
    [SerializeField] float horizontalAngle = 0;  // 카메라Arm 수평각도(수 올리면 카메라 왼쪽, 시야는 오른쪽) 

    public float onShiftTime { get; private set; } = 0f; // 달리기 누르고 몇초됬는지
    public bool onShift { get; private set; }           // 좌쉬프트 눌렀는지

    public Vector3 moveDir { get; private set; } // 이동 방향(normalized된)

    bool isAttacking = false; // 공격중인지

    bool _isGrounded;
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }

    void Start()
    {
        // state할당
        States[(int)PlayerState.Idle] = new IdleState();
        States[(int)PlayerState.Walk] = new WalkState();
        States[(int)PlayerState.Run] = new RunState();
        States[(int)PlayerState.Attack] = new AttackState();
        States[(int)PlayerState.Jump] = new JumpState();
        States[(int)PlayerState.Dodge] = new DodgeState();
        States[(int)PlayerState.Hit] = new HitState();
        // 초기상태 할당
        currentState = States[(int)PlayerState.Idle];

        anim = appearance.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        stat = GetComponent<PlayerStatus>();
    }
    void Update()
    {
        // 카메라가 보는 방향이 앞이되도록 이동시킴
        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0, cameraArm.right.z).normalized;
        moveDir = lookForward * InputManager.moveDirection.y + lookRight * InputManager.moveDirection.x;

        // 현재 상태 업데이트
        currentState.Execute(this);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (onShift != true)
                onShift = true; // 좌쉬프트 타임 체크
        };
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            onShift = false;
            onShiftTime = 0f;
        };

        if (Input.GetKeyDown(KeyCode.I))
        {
            CloserableUIBase.ToggleUI("Inventory");
            mouseFix = false; // 마우스 잠금도 풀거야
        };

        if (Input.GetKeyDown(KeyCode.Escape)) mouseFix = false; // ESC키 누를 시 마우스 잠금 해제

        if (mouseFix && InputManager.mouseChangedPosition.magnitude > 0) // 마우스가 고정되어있고 움직인다면
        {
            Vector3 cameraArmAngle = cameraArm.rotation.eulerAngles;

            verticalAngle -= InputManager.mouseChangedPosition.y * InputManager.mouseSensitive; // 마우스 올리면 카메라 내려가게
            horizontalAngle += InputManager.mouseChangedPosition.x * InputManager.mouseSensitive; // 마우스 오른쪽 -> 카메라 왼쪽

            verticalAngle = Mathf.Clamp(verticalAngle, -50f, 50f); // 수직회전 제한

            cameraArm.rotation = Quaternion.Euler(verticalAngle, horizontalAngle, cameraArmAngle.z);
        }

        #region InteractBase 상호작용
        RaycastHit hit;
        // 카메라 가운데서 쏘도록
        Ray currentRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        currentRay.origin = cameraArm.position; // 캐릭터위치에서 쏘기
        Physics.Raycast(currentRay, out hit, 2, 1 << LayerMask.NameToLayer("Interact"));

        if (hit.transform)
        {
            if (hit.transform != lastHit) // 이전거랑 다르면
            {
                interactFocus = hit.transform.GetComponent<InteractBase>();
            };
        }
        else
        {
            interactFocus = null; // 못찾음
        };
        lastHit = hit.transform; // 마지막으로 hit한 녀석 저장

        #endregion InteractBase 상호작용

        if (Input.GetKeyDown(KeyCode.E) && interactFocus) // 상호작용 대상이 있을때 E키를 누르면
        {
            interactFocus.Interact();
        };

        CheckShift();

    }

    private void LateUpdate()
    {
        if(moveDir.magnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(moveDir);
            appearance.transform.rotation = Quaternion.Lerp(appearance.transform.rotation, rot, 0.1f);
        }
    }

    // 상태를 변경해주는 메서드
    public void ChangeState(PlayerStateBase state) 
    {
        currentState.Exit(this);
        currentState = state;
        currentState.Enter(this);
    }

    // 움직이는거
    public void Move(Vector3 moveDir)
    {
        //      최종 이동량         방향         *   속도가 0보다 작아지지않도록       밀리는힘, 방향
        Vector3 totalDirection = (moveDir * Mathf.Max(stat.MoveSpeed, 0));
        totalDirection *= Time.deltaTime; // 초당 움직이도록
        transform.position += totalDirection; // 최종 계산된 이동량만큼 이동
    }

    public virtual void CheckShift()
    {
        if (onShift)
        {
            onShiftTime += Time.deltaTime;
        };
    }

    void Dead()
    {
        GameManager.OnPlayerDead(); // 캐릭터 사망시 작동
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "MonsterAttack")
        {
            CharacterBase from = other.gameObject.GetComponentInParent<CharacterBase>();
            ApplyDamage(from.stat.AttackDamage, from);
        }

        if (other.transform.tag == "Ground")
        {
            IsGrounded = true;
            if (currentState == States[(int)PlayerState.Jump])
                ChangeState(States[(int)PlayerState.Idle]);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Ground")
        {
            IsGrounded = false;
            if (currentState != States[(int)PlayerState.Jump])
                ChangeState(States[(int)PlayerState.Jump]);

        }
    }

    private void ApplyDamage(float damage, CharacterBase from)
    {
        damage = Mathf.Min(damage, stat.HealthCurrent); // 생명력 이상의 데미지 제한

        if (damage == 0) return;  //데미지가 0이라면 데미지 안받은 걸로 치기(??)_

        stat.HealthCurrent -= damage;

        ChangeState(States[(int)PlayerState.Hit]);

        if (stat.HealthCurrent <= 0) // 죽었다면
        {
            Dead();
        }
    }

}

