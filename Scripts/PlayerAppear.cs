using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAppear : MonoBehaviour
{
    public PlayerBase owner;
    public Collider weaponCol;

    private void Start()
    {
        owner = GetComponentInParent<PlayerBase>();
    }

    public void OnDodge()
    {

    }

    public void ExitDodge()
    {
        owner.ChangeState(owner.States[(int)PlayerState.Idle]);
    }


    public void OnAttack()
    {

    }

    public void ExitAttack()
    {
        owner.ChangeState(owner.States[(int)PlayerState.Idle]);
    }

    public void ExitHit()
    {
        owner.ChangeState(owner.States[(int)PlayerState.Idle]);
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
