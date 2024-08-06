using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateMachine;

[RequireComponent(typeof(PlayerStatus))]
public class PlayerBase : MonoBehaviour
{
    public static InteractBase interactFocus; // ���� �����ִ� ��ȣ�ۿ� ���
    public static bool mouseFix // ���콺 ����

    {
        get
        {
            return Cursor.lockState == CursorLockMode.Locked; // ���콺�� ������ ��������
        }

        set
        {
            if (value) // ���(���콺 ����)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else // �������(���콺 ����)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public GameObject appearance;

    public PlayerStateBase[] States = new PlayerStateBase[(int)PlayerState.Length]; // ĳ������ ���¸ӽŵ� �迭
    public PlayerStateBase currentState; // ���� ĳ������ ����

    public Animator     anim { get; private set; }
    public PlayerStatus stat { get; private set; }
    public Rigidbody    rigid { get; private set; }

    /// <summary> �� ĳ���Ͱ� ���ϰ��ִ� ī�޶�ȸ��ü </summary>
    public Transform cameraArm; 

    Transform lastHit;      // ���������� ���� �༮�� ì���

    [SerializeField] float verticalAngle = 15; // ī�޶�Arm ��������(�� �ø��� ī�޶� ����, �þߴ� �Ʒ���)
    [SerializeField] float horizontalAngle = 0;  // ī�޶�Arm ���򰢵�(�� �ø��� ī�޶� ����, �þߴ� ������) 

    public float onShiftTime { get; private set; } = 0f; // �޸��� ������ ���ʉ����
    public bool onShift { get; private set; }           // �½���Ʈ ��������

    public Vector3 moveDir { get; private set; } // �̵� ����(normalized��)

    bool isAttacking = false; // ����������

    bool _isGrounded;
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }

    void Start()
    {
        // state�Ҵ�
        States[(int)PlayerState.Idle] = new IdleState();
        States[(int)PlayerState.Walk] = new WalkState();
        States[(int)PlayerState.Run] = new RunState();
        States[(int)PlayerState.Attack] = new AttackState();
        States[(int)PlayerState.Jump] = new JumpState();
        States[(int)PlayerState.Dodge] = new DodgeState();
        States[(int)PlayerState.Hit] = new HitState();
        // �ʱ���� �Ҵ�
        currentState = States[(int)PlayerState.Idle];

        anim = appearance.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        stat = GetComponent<PlayerStatus>();
    }
    void Update()
    {
        // ī�޶� ���� ������ ���̵ǵ��� �̵���Ŵ
        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0, cameraArm.right.z).normalized;
        moveDir = lookForward * InputManager.moveDirection.y + lookRight * InputManager.moveDirection.x;

        // ���� ���� ������Ʈ
        currentState.Execute(this);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (onShift != true)
                onShift = true; // �½���Ʈ Ÿ�� üũ
        };
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            onShift = false;
            onShiftTime = 0f;
        };

        if (Input.GetKeyDown(KeyCode.I))
        {
            CloserableUIBase.ToggleUI("Inventory");
            mouseFix = false; // ���콺 ��ݵ� Ǯ�ž�
        };

        if (Input.GetKeyDown(KeyCode.Escape)) mouseFix = false; // ESCŰ ���� �� ���콺 ��� ����

        if (mouseFix && InputManager.mouseChangedPosition.magnitude > 0) // ���콺�� �����Ǿ��ְ� �����δٸ�
        {
            Vector3 cameraArmAngle = cameraArm.rotation.eulerAngles;

            verticalAngle -= InputManager.mouseChangedPosition.y * InputManager.mouseSensitive; // ���콺 �ø��� ī�޶� ��������
            horizontalAngle += InputManager.mouseChangedPosition.x * InputManager.mouseSensitive; // ���콺 ������ -> ī�޶� ����

            verticalAngle = Mathf.Clamp(verticalAngle, -50f, 50f); // ����ȸ�� ����

            cameraArm.rotation = Quaternion.Euler(verticalAngle, horizontalAngle, cameraArmAngle.z);
        }

        #region InteractBase ��ȣ�ۿ�
        RaycastHit hit;
        // ī�޶� ����� ���
        Ray currentRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        currentRay.origin = cameraArm.position; // ĳ������ġ���� ���
        Physics.Raycast(currentRay, out hit, 2, 1 << LayerMask.NameToLayer("Interact"));

        if (hit.transform)
        {
            if (hit.transform != lastHit) // �����Ŷ� �ٸ���
            {
                interactFocus = hit.transform.GetComponent<InteractBase>();
            };
        }
        else
        {
            interactFocus = null; // ��ã��
        };
        lastHit = hit.transform; // ���������� hit�� �༮ ����

        #endregion InteractBase ��ȣ�ۿ�

        if (Input.GetKeyDown(KeyCode.E) && interactFocus) // ��ȣ�ۿ� ����� ������ EŰ�� ������
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

    // ���¸� �������ִ� �޼���
    public void ChangeState(PlayerStateBase state) 
    {
        currentState.Exit(this);
        currentState = state;
        currentState.Enter(this);
    }

    // �����̴°�
    public void Move(Vector3 moveDir)
    {
        //      ���� �̵���         ����         *   �ӵ��� 0���� �۾������ʵ���       �и�����, ����
        Vector3 totalDirection = (moveDir * Mathf.Max(stat.MoveSpeed, 0));
        totalDirection *= Time.deltaTime; // �ʴ� �����̵���
        transform.position += totalDirection; // ���� ���� �̵�����ŭ �̵�
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
        GameManager.OnPlayerDead(); // ĳ���� ����� �۵�
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
        damage = Mathf.Min(damage, stat.HealthCurrent); // ����� �̻��� ������ ����

        if (damage == 0) return;  //�������� 0�̶�� ������ �ȹ��� �ɷ� ġ��(??)_

        stat.HealthCurrent -= damage;

        ChangeState(States[(int)PlayerState.Hit]);

        if (stat.HealthCurrent <= 0) // �׾��ٸ�
        {
            Dead();
        }
    }

}

