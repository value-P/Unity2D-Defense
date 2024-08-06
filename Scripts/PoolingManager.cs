using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public GameObject prefab;
    public int poolingCount;    // �ѹ��� �󸶳� �����ϴ���
}

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public static List<InteractItem> dropBoxList = new List<InteractItem>(); // ����ǰ���� ������ƮǮ
    public Pool dropBox;
    public Transform dropBoxParent;

    public static List<MonsterBase> monsterList = new List<MonsterBase>(); // ���͵��� ��ġ�� ��Ƴ��� ����Ʈ
    //public Pool monster;
    //public Transform monsterParent;


    void Start()
    {
        InitializePool(dropBox, dropBoxParent, dropBoxList);
        //InitializePool(monster, monsterParent, monsterList);
    }

    private void InitializePool<T>(Pool wantPooling, Transform parent, List<T> wantList) where T : class // �ϴ� �䷸��
    {
        for(int i = 0; i < wantPooling.poolingCount; i++)
        {
            GameObject obj = Instantiate(wantPooling.prefab, parent);
            obj.SetActive(false);
            T inst = obj.GetComponent<T>();
            wantList.Add(inst);
        };
    }

    public static InteractItem GetDropBox() // ��Ȱ��ȭ�� �ڽ� ��������
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
