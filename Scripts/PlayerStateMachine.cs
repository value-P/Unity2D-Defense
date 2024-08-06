using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class IdleState : PlayerStateBase
    {
        public override void Enter(PlayerBase target)
        {
            target.anim.CrossFade("Idle", 0.2f);
        }

        public override void Execute(PlayerBase target)
        {
            if (target.moveDir.magnitude > 0.1f)
                target.ChangeState(target.States[(int)PlayerState.Walk]);

            if (InputManager.GetKeyState(MouseCode.LeftClick) == KeyState.Down)
            {
                if (PlayerBase.mouseFix) // ���콺�� �����Ǿ��ִٸ�
                {
                    target.ChangeState(target.States[(int)PlayerState.Attack]);
                }
                else if (InputManager.mouseHitInterface == null) // UI���� ���콺�� ���ٸ�
                {
                    PlayerBase.mouseFix = true;
                };
            };

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                if (target.onShiftTime <= 0.2f)
                    target.ChangeState(target.States[(int)PlayerState.Dodge]);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!target.IsGrounded) return;
                target.rigid.AddForce(Vector3.up * target.stat.JumpPower);
            }
        }

        public override void Exit(PlayerBase target)
        {
            
        }
    }

    public class WalkState : PlayerStateBase
    {
        public override void Enter(PlayerBase target)
        {
            target.stat.moveSpeedMultiplier = 1f; // �ӵ� ����ȭ
            target.anim.CrossFade("Walk", 0.2f);
        }

        public override void Execute(PlayerBase target)
        {
            target.Move(target.moveDir);

            if (target.moveDir.magnitude < 0.1f)
                target.ChangeState(target.States[(int)PlayerState.Idle]);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (target.onShiftTime > 0.2f)
                    target.ChangeState(target.States[(int)PlayerState.Run]);
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                if (target.onShiftTime <= 0.2f)
                    target.ChangeState(target.States[(int)PlayerState.Dodge]);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!target.IsGrounded) return;
                target.rigid.AddForce(Vector3.up * target.stat.JumpPower);
            }

        }

        public override void Exit(PlayerBase target)
        {
            
        }
    }

    public class RunState : PlayerStateBase
    {
        public override void Enter(PlayerBase target)
        {
            target.stat.moveSpeedMultiplier = 1.5f; // �ӵ� ��
            target.anim.CrossFade("Run", 0.2f);
        }

        public override void Execute(PlayerBase target)
        {
            target.Move(target.moveDir);

            if (target.moveDir.magnitude < 0.2f)
                target.ChangeState(target.States[(int)PlayerState.Idle]);

            if (Input.GetKeyUp(KeyCode.LeftShift))
                target.ChangeState(target.States[(int)PlayerState.Walk]);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!target.IsGrounded) return;
                target.rigid.AddForce(Vector3.up * target.stat.JumpPower);
            }

        }

        public override void Exit(PlayerBase target)
        {
        }
    }

    public class AttackState : PlayerStateBase
    {
        public override void Enter(PlayerBase target)
        {
            target.anim.CrossFade("Attack1", 0.2f);
        }

        public override void Execute(PlayerBase target)
        {
            if (Input.GetMouseButtonDown(0))
                target.anim.SetTrigger("Attack");
                
            // ���Է�ó�� TODO
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                if (target.onShiftTime <= 0.2f)
                    target.ChangeState(target.States[(int)PlayerState.Dodge]);
            }
        }

        public override void Exit(PlayerBase target)
        {

        }
    }

    public class JumpState : PlayerStateBase
    {
        public override void Enter(PlayerBase target)
        {
            target.anim.CrossFade("Jump", 0.2f);
        }

        public override void Execute(PlayerBase target)
        {
            target.Move(target.moveDir);
        }

        public override void Exit(PlayerBase target)
        {
        }
    }

    public class DodgeState : PlayerStateBase
    {
        Vector3 dodgeDir; // ������ ���� ����

        public override void Enter(PlayerBase target)
        {
            // ���� ������ ���⺤�ͱ������� ��������(���� ������ ���¹�������)
            if (target.moveDir.magnitude > 0.1f)
            {
                dodgeDir = target.moveDir;
            }
            else
            {
                dodgeDir = target.appearance.transform.forward;
            }
            
            target.anim.Play("Dodge");
        }

        public override void Execute(PlayerBase target)
        {
            // ù �Է¹��� �������θ� ������
            target.transform.position += dodgeDir * target.stat.DodgePower * Time.deltaTime;
        }

        public override void Exit(PlayerBase target)
        {
        }
    }

    public class HitState : PlayerStateBase
    {
        public override void Enter(PlayerBase target)
        {
            target.anim.CrossFade("Hit", 0.2f);
        }

        public override void Execute(PlayerBase target)
        {
        }

        public override void Exit(PlayerBase target)
        {
        }
    }


}
