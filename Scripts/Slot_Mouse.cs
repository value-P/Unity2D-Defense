using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot_Mouse : SlotBase
{
    protected override void Start()
    {
        SetContainer(InventoryBase.mouseContainer); // ���콺 �����̳ʷ� ����
    }

    protected override void Update()
    {
        base.Update();
        transform.position = Input.mousePosition; // ���콺 ��ġ�� �̵�
    }
}
