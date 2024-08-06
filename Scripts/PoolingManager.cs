using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public GameObject prefab;
    public int poolingCount;    // 한번에 얼마나 생성하는지
}

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public static List<InteractItem> dropBoxList = new List<InteractItem>(); // 전리품상자 오브젝트풀
    public Pool dropBox;
    public Transform dropBoxParent;

    public static List<MonsterBase> monsterList = new List<MonsterBase>(); // 몬스터들의 위치를 담아놓을 리스트
    //public Pool monster;
    //public Transform monsterParent;


    void Start()
    {
        InitializePool(dropBox, dropBoxParent, dropBoxList);
        //InitializePool(monster, monsterParent, monsterList);
    }

    private void InitializePool<T>(Pool wantPooling, Transform parent, List<T> wantList) where T : class // 일단 요렇게
    {
        for(int i = 0; i < wantPooling.poolingCount; i++)
        {
            GameObject obj = Instantiate(wantPooling.prefab, parent);
            obj.SetActive(false);
            T inst = obj.GetComponent<T>();
            wantList.Add(inst);
        };
    }

    public static InteractItem GetDropBox() // 비활성화된 박스 가져오기
    {
        foreach(var current in dropBoxList)
        {
            if(!current.gameObject.activeInHierarchy)
            {
                return current;
            }
        }

        return null;
    }

    public static void SpawnMonster(List<MonsterBase> wantList, Vector3 wantPos)
    {
        foreach(var current in wantList)
        {
            if(!current.gameObject.activeInHierarchy)
            {
                current.transform.position = wantPos;
                current.gameObject.SetActive(true); 
                UI_Manager.Instance.MonsterHpBarSet(current);

                return;
            }
        }

    }
}
