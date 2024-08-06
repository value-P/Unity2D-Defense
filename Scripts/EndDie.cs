using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDie : StateMachineBehaviour
{
    //OnStateExit is called when a transition ends and the State machine finishes evaluating this State
    override public void OnStateExit(Animator animator, AnimatorStateInfo StateInfo, int layerIndex)
    {
        MonsterBase target = animator.transform.GetComponentInParent<MonsterBase>();

        // ������ ������ ��� , ������Ʈ Ǯ���� ���� ��Ȱ��ȭ
        target.gameObject.SetActive(false);
        InteractItem itembox = PoolingManager.GetDropBox();
        itembox.SetItemList(target.dropList);
        itembox.gameObject.transform.position = target.transform.position;
        itembox.gameObject.SetActive(true);

    }

}
