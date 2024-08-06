using System;
using UnityEngine;

namespace MonsterStateMachine
{
    // ��������
    public class PatrolState : MonsterStateBase
    {
        protected Vector3 lastDest = Vector3.zero; // ���� �ֱ��� ������

        public override void Enter(MonsterBase target)
        {
            MoveRandomPoint(target);
        }

        public override void Execute(MonsterBase target)
        {
            MoveRandomPoint(target);
            DetectPlayer(target);
        }

        public override void Exit(MonsterBase target)
        {
            target.agent.SetDestination(target.transform.position);
        }

        protected virtual void MoveRandomPoint(MonsterBase target) // ���� ��ġ�� �̵��ϴ� �޼���
        {
            if(lastDest == Vector3.zero || Vector3.Distance(target.transform.position, lastDest) <= target.agent.stoppingDistance)
            {
                Vector3 randPos = target.originPosition.RandomPoint(target.maxAreaDistance);
                target.agent.SetDestination(randPos);
                lastDest = randPos;
            }
        }

        protected virtual void DetectPlayer(MonsterBase target)
        {
            Vector3 dist = target.player.transform.position - target.transform.position;
            if (dist.magnitude > target.viewDistance) return; // �ִ� �þ߰Ÿ����� �ָ� �ִٸ� ���� �ȵ�

            float distAngle = Vector3.Angle(dist.normalized, target.transform.forward);
            // ������ ������ �������� ���� 0~180���� ���´� �� �� ���� ���� �þ߰����� �۴ٸ� �þ߿� ���°�

            if (distAngle <= target.viewAngle * 0.5f) // ���� �þ߰��� ���ݺ��� ���� ũ�ٸ� �þ߿� �����Ƿ� ���� �ȵ�
            {
                RaycastHit hit; // �浹����
                Ray currentRay = new Ray();
                currentRay.origin = target.transform.position;
                currentRay.direction = dist.normalized;

                Physics.Raycast(currentRay, out hit, target.viewDistance);

                if (hit.transform.tag == "Player")
                {
                    target.focusTarget = hit.transform; // Ÿ�� ���
                    if (dist.magnitude <= target.attackDistance)
                    {
                        target.ChangeState(target.States[(int)MonsterState.Combat]); // ĳ���Ͱ� �ε����ٸ� �������� ����
                    }
                    else
                    {
                        target.ChangeState(target.States[(int)MonsterState.Chase]); // ĳ���Ͱ� �ε����ٸ� �������� ����
                    }
                }
                else return; // �� �ܿ��� ���� ����
            }
        }

    }

    // �߰ݻ���
    public class ChaseState : MonsterStateBase
    {
        public override void Enter(MonsterBase target)
        {
            target.agent.SetDestination(target.player.transform.position);
        }

        public override void Execute(MonsterBase target)
        {
            float dist = Vector3.Distance(target.player.transform.position , target.transform.position);
            if (dist <= target.attackDistance)
                target.ChangeState(target.States[(int)MonsterState.Combat]);
            else
            {
                target.agent.SetDestination(target.player.transform.position);
            }

        }

        public override void Exit(MonsterBase target)
        {
            target.agent.SetDestination(target.transform.position);
        }
    }

    // ���ݻ���
    public class CombatState : MonsterStateBase
    {
        float attackDelay;
        float currentTime = 0f;

        public override void Enter(MonsterBase target)
        {
            attackDelay = target.stat.AttackDelay;    
        }

        public override void Execute(MonsterBase target)
        {
            Vector3 direction = target.player.transform.position - target.transform.position;
            float dist = direction.magnitude;


            if(!target.isAttacking)
            {
                Vector3 vec = new Vector3(direction.x, 0, direction.z).normalized;
                target.transform.rotation = Quaternion.Lerp(target.transform.rotation ,Quaternion.LookRotation(vec), 0.1f);

                if (dist > target.attackDistance)
                    target.ChangeState(target.States[(int)MonsterState.Chase]);
            }

            if (attackDelay <= currentTime)
            {
                RandomAttack(target);
                currentTime = 0f;
            }
            currentTime += Time.deltaTime;
        }

        public override void Exit(MonsterBase target)
        {
        }

        private void RandomAttack(MonsterBase target)
        {
            int rand = UnityEngine.Random.Range(0, target.randomAttackNum);

            target.anim?.SetInteger("AttackNum", rand);
            target.anim?.SetTrigger("Attack");
        }
    }

}
