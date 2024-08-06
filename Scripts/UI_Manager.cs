using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager _instance;
    public static UI_Manager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameManager.Instance.gameObject.AddComponent<UI_Manager>();

            return _instance;
        }
    }
    private void Awake()
    {
        this.Singleton(ref _instance);
    }

    protected static Canvas _mainCanvas = null;
    public static Canvas MainCanvas // ���� ĵ����
    {
        get
        {
            if(!_mainCanvas) // ��������� �±׷� ã�Ƽ� ĵ���� ������Ʈ �־��ֱ�
            {
                _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas")?.GetComponent<Canvas>();

                //�׷��� ���µ�? <-- ���� �� ���ҽ� ����
                if (!_mainCanvas)
                {
                    _mainCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/UserInterface/MainUserInterface"))?.GetComponent<Canvas>();
                };
            };

            return _mainCanvas;
        }
    }
    
    protected static RectTransform objectLeftTop = null;
    protected static RectTransform objectRightBottom = null;
    public static Vector3 screenLeftTop
    {
        get
        {
            if(!objectLeftTop) // �»� ������ ������
            {
                objectLeftTop = new GameObject("LeftTop").AddComponent<RectTransform>();
                objectLeftTop.SetParent(MainCanvas.transform);
                // ��Ŀ�� ���� ����
                objectLeftTop.anchorMin = new Vector2(0, 1);
                objectLeftTop.anchorMax = new Vector2(0, 1);
                // ���� ���߱�
                objectLeftTop.anchoredPosition = Vector3.zero;
            }
            return objectLeftTop.position;
        }
    }
    public static Vector3 screenRightBottom
    {
        get
        {
            if(!objectRightBottom) // �»� ������ ������
            {
                objectRightBottom = new GameObject("LeftTop").AddComponent<RectTransform>();
                objectRightBottom.SetParent(MainCanvas.transform);
                // ��Ŀ�� ���� ����
                objectRightBottom.anchorMin = new Vector2(1, 0);
                objectRightBottom.anchorMax = new Vector2(1, 0);
                // ���� ���߱�
                objectRightBottom.anchoredPosition = Vector3.zero;
            }
            return objectRightBottom.position;
        }
    }

    public static string currentFocusUI; // ���� �����ִ� UI

    public InventoryBase dropBoxUI; // ����ǰ���� UI

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public Transform monsterHpParent;
    public GameObject monsterHP_Prefab; // ���� ü�¹� ������
    public List<MonsterHP> monsterHPList = new List<MonsterHP>(); // ���� ü�¹ٸ� ���������� ����Ʈ
    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    private void Start()
    {
        InstantiateMonsterHpUI(); // �⺻ 10�� ����
    }

    public virtual void ClaimUI(string name, bool open) { currentFocusUI = name; }
    public void ClaimUI(string name) { ClaimUI(name, true); }


    //////////////////////////////////////////////////////////////////////////////////////////////////
    public void InstantiateMonsterHpUI() // ���� ü�¹ٸ� ����� �޼���(10��)
    {
        for(int i = 0; i < 10; i++)
        {
            // ��ü ���� �� ����(��Ȱ��ȭ,���̾��Ű���� ��Ƴ���)
            GameObject monsterHpBar = Instantiate(monsterHP_Prefab);
            monsterHpBar.SetActive(false);
            monsterHpBar.transform.SetParent(monsterHpParent);
            MonsterHP hP = monsterHpBar.GetComponent<MonsterHP>();
            monsterHPList.Add(hP); //����Ʈ�� �߰�
        }
    }

    public void MonsterHpBarSet(MonsterBase target) // ���� ������ ü�¹� �Ҵ����ֵ���
    {
        foreach(var current in monsterHPList)
        {
            if(!current.gameObject.activeInHierarchy && current.ownerMonster == null) // ��Ȱ��ȭ �Ǿ��ִٸ� && ���� ���ٸ�
            {
                target.hpBar = current; // �������׵� ü�¹� �˷���
                current.ownerMonster = target; // ��������
                current.gameObject.SetActive(true);
                return;
            };
        };

        // ������� �Դٸ� ���� ü�¹ٰ� ���ٴ� �� �̹Ƿ� �߰�
        InstantiateMonsterHpUI();
        // ���� �������ϱ� �ٽ� ����
        foreach (var current in monsterHPList)
        {
            if (!current.gameObject.activeInHierarchy && current.ownerMonster == null) 
            {
                target.hpBar = current; // �������׵� ü�¹� �˷���
                current.ownerMonster = target; // ��������
                current.gameObject.SetActive(true);
                return;
            };
        };

    }
    //////////////////////////////////////////////////////////////////////////////////////////////////
}
