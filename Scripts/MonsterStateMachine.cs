using System;
using UnityEngine;

namespace MonsterStateMachine
{
    // 순찰상태
    public class PatrolState : MonsterStateBase
    {
        protected Vector3 lastDest = Vector3.zero; // 가장 최근의 목적지

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

        protected virtual void MoveRandomPoint(MonsterBase target) // 랜덤 위치로 이동하는 메서드
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
            if (dist.magnitude > target.viewDistance) return; // 최대 시야거리보다 멀리 있다면 실행 안됨

            float distAngle = Vector3.Angle(dist.normalized, target.transform.forward);
            // 몬스터의 정면을 기준으로 갈라서 0~180도가 나온다 즉 이 값이 정한 시야각보다 작다면 시야에 들어온것

            if (distAngle <= target.viewAngle * 0.5f) // 만약 시야각의 절반보다 값이 크다면 시야에 없으므로 실행 안됨
            {
                RaycastHit hit; // 충돌정보
                Ray currentRay = new Ray();
                currentRay.origin = target.transform.position;
                currentRay.direction = dist.normalized;

                Physics.Raycast(currentRay, out hit, target.viewDistance);

                if (hit.transform.tag == "Player")
                {
                    target.focusTarget = hit.transform; // 타겟 등록
                    if (dist.magnitude <= target.attackDistance)
                    {
                        target.ChangeState(target.States[(int)MonsterState.Combat]); // 캐릭터가 부딪혔다면 전투상태 돌입
                    }
                    else
                    {
                        target.ChangeState(target.States[(int)MonsterState.Chase]); // 캐릭터가 부딪혔다면 전투상태 돌입
                    }
                }
                else return; // 그 외에는 반응 없음
            }
        }

    }

    // 추격상태
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

    // 공격상태
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
