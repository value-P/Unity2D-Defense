using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHP : MonoBehaviour
{
    public MonsterBase ownerMonster; // 체력바 주인장
    public Slider mySlider; // 내 슬라이더

    private void Start()
    {
        mySlider = GetComponent<Slider>();
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(ownerMonster.transform.position + new Vector3(0, 3, 0)); // 주인 머리위에 이동
        if(transform.position.z < 0.0f)
        {
            transform.position *= -1f;
        }
        mySlider.value = ownerMonster.stat.healthRate; // 수치 업데이트
    }

}
