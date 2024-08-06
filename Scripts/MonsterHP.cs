using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHP : MonoBehaviour
{
    public MonsterBase ownerMonster; // ü�¹� ������
    public Slider mySlider; // �� �����̴�

    private void Start()
    {
        mySlider = GetComponent<Slider>();
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(ownerMonster.transform.position + new Vector3(0, 3, 0)); // ���� �Ӹ����� �̵�
        if(transform.position.z < 0.0f)
        {
            transform.position *= -1f;
        }
        mySlider.value = ownerMonster.stat.healthRate; // ��ġ ������Ʈ
    }

}
