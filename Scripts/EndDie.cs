using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDie : StateMachineBehaviour
{
    //OnStateExit is called when a transition ends and the State machine finishes evaluating this State
    override public void OnStateExit(Animator animator, AnimatorStateInfo StateInfo, int layerIndex)
    {
        MonsterBase target = animator.transform.GetComponentInParent<MonsterBase>();

        // 죽으면 아이템 드랍 , 오브젝트 풀링을 위한 비활성화
        target.gameObject.SetActive(false);
        InteractItem itembox = PoolingManager.GetDropBox();
        itembox.SetItemList(target.dropList);
        itembox.gameObject.transform.position = target.transform.position;
        itembox.gameObject.SetActive(true);

    }

}
