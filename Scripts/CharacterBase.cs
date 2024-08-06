using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStatus))] //캐릭터는 스테이터스가 항상 붙어있도록 한다
public abstract class CharacterBase : MonoBehaviour
{
    [HideInInspector] public CharacterStatus stat;    // 스테이터스
    [HideInInspector] public Animator anim;
    public GameObject appearance;


    /// <summary> 이 캐릭터가 집중하고 있는 대상 </summary>
    [HideInInspector] public Transform focusTarget;
    System.Action CharacterUpdate;    // 업데이트 통합

    protected virtual void Start()
    {
        stat = GetComponent<CharacterStatus>();
        stat.owner = this;

        CharacterUpdate += MovementUpdate;       // 움직임 업데이트 추가

        if (appearance) // 보여주는 대상이 있다면 컴포넌트 받아오기(애니메이터와 appear인포)
        {
            anim = appearance.GetComponent<Animator>();
        };

        if (anim != null) CharacterUpdate += AnimationUpdate;           //애니메이션 업데이트 추가
    }


    protected virtual void Update()
    {
        if (CharacterUpdate != null) CharacterUpdate(); // 캐릭터업데이트 비어있지 않다면 실행
    }

    protected abstract void MovementUpdate(); // 이동 업데이트!! 
    protected abstract void AnimationUpdate();
    public abstract void MoveTo(Vector3 wantValue); // 이동방향 설정 메서드
    public abstract void Attack();
    public virtual float ApplyDamage(float damage, PlayerBase from) // 캐릭터에게 데미지를 주는 메서드
    {
        damage = Mathf.Min(damage, stat.HealthCurrent); // 생명력 이상의 데미지 제한

        if (damage == 0) return 0;  //데미지가 0이라면 데미지 안받은 걸로 치기(??)_

        stat.HealthCurrent -= damage;

        anim?.SetTrigger("Hit"); // 데미지 애니메이션

        if (stat.HealthCurrent <= 0) // 죽었다면
        {
            Dead();
        }

        return damage;
    }
    public abstract void Dead();

}