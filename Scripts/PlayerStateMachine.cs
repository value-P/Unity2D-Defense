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
                if (PlayerBase.mouseFix) // 마우스가 고정되어있다면
                {
                    target.ChangeState(target.States[(int)PlayerState.Attack]);
                }
                else if (InputManager.mouseHitInterface == null) // UI위에 마우스가 없다면
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
            target.stat.moveSpeedMultiplier = 1f; // 속도 정상화
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
            target.stat.moveSpeedMultiplier = 1.5f; // 속도 업
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
                
            // 선입력처리 TODO
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
        Vector3 dodgeDir; // 구르기 방향 벡터

        public override void Enter(PlayerBase target)
        {
            // 상태 들어갈때의 방향벡터기준으로 구르도록(방향 없으면 보는방향으로)
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
            // 첫 입력받은 방향으로만 구르기
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
