using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAppear : MonoBehaviour
{
    public MonsterBase owner;
    public Collider weaponCol;

    private void Start()
    {
        owner = GetComponentInParent<MonsterBase>();
    }

    public void OnDodge()
    {
    }

    public void ExitDodge()
    {
    }


    public void OnAttack()
    {
        owner.isAttacking = true;
    }

    public void ExitAttack()
    {
        owner.isAttacking = false;
    }

    public void OnWeaponCol()
    {
        weaponCol.enabled = true;
    }

    public void ExitWeaponCol()
    {
        weaponCol.enabled = false;
    }

}
