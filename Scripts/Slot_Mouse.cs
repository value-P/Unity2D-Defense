using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot_Mouse : SlotBase
{
    protected override void Start()
    {
        SetContainer(InventoryBase.mouseContainer); // 마우스 컨테이너로 설정
    }

    protected override void Update()
    {
        base.Update();
        transform.position = Input.mousePosition; // 마우스 위치로 이동
    }
}
