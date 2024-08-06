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
    public static Canvas MainCanvas // 메인 캔버스
    {
        get
        {
            if(!_mainCanvas) // 비어있으면 태그로 찾아서 캔버스 컴포넌트 넣어주기
            {
                _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas")?.GetComponent<Canvas>();

                //그래도 없는데? <-- 현재 이 리소스 없다
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
            if(!objectLeftTop) // 좌상 꼭짓점 없으면
            {
                objectLeftTop = new GameObject("LeftTop").AddComponent<RectTransform>();
                objectLeftTop.SetParent(MainCanvas.transform);
                // 앵커를 좌측 위로
                objectLeftTop.anchorMin = new Vector2(0, 1);
                objectLeftTop.anchorMax = new Vector2(0, 1);
                // 영점 맞추기
                objectLeftTop.anchoredPosition = Vector3.zero;
            }
            return objectLeftTop.position;
        }
    }
    public static Vector3 screenRightBottom
    {
        get
        {
            if(!objectRightBottom) // 좌상 꼭짓점 없으면
            {
                objectRightBottom = new GameObject("LeftTop").AddComponent<RectTransform>();
                objectRightBottom.SetParent(MainCanvas.transform);
                // 앵커를 좌측 위로
                objectRightBottom.anchorMin = new Vector2(1, 0);
                objectRightBottom.anchorMax = new Vector2(1, 0);
                // 영점 맞추기
                objectRightBottom.anchoredPosition = Vector3.zero;
            }
            return objectRightBottom.position;
        }
    }

    public static string currentFocusUI; // 현재 보고있는 UI

    public InventoryBase dropBoxUI; // 전리품상자 UI

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public Transform monsterHpParent;
    public GameObject monsterHP_Prefab; // 몬스터 체력바 프리팹
    public List<MonsterHP> monsterHPList = new List<MonsterHP>(); // 몬스터 체력바를 가지고있을 리스트
    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    private void Start()
    {
        InstantiateMonsterHpUI(); // 기본 10개 생성
    }

    public virtual void ClaimUI(string name, bool open) { currentFocusUI = name; }
    public void ClaimUI(string name) { ClaimUI(name, true); }


    //////////////////////////////////////////////////////////////////////////////////////////////////
    public void InstantiateMonsterHpUI() // 몬스터 체력바를 만드는 메서드(10개)
    {
        for(int i = 0; i < 10; i++)
        {
            // 객체 생성 후 설정(비활성화,하이어라키에서 모아놓기)
            GameObject monsterHpBar = Instantiate(monsterHP_Prefab);
            monsterHpBar.SetActive(false);
            monsterHpBar.transform.SetParent(monsterHpParent);
            MonsterHP hP = monsterHpBar.GetComponent<MonsterHP>();
            monsterHPList.Add(hP); //리스트에 추가
        }
    }

    public void MonsterHpBarSet(MonsterBase target) // 몬스터 생성시 체력바 할당해주도록
    {
        foreach(var current in monsterHPList)
        {
            if(!current.gameObject.activeInHierarchy && current.ownerMonster == null) // 비활성화 되어있다면 && 주인 없다면
            {
                target.hpBar = current; // 몬스터한테도 체력바 알려줌
                current.ownerMonster = target; // 주인지정
                current.gameObject.SetActive(true);
                return;
            };
        };

        // 여기까지 왔다면 남은 체력바가 없다는 뜻 이므로 추가
        InstantiateMonsterHpUI();
        // 새로 들어왔으니까 다시 실행
        foreach (var current in monsterHPList)
        {
            if (!current.gameObject.activeInHierarchy && current.ownerMonster == null) 
            {
                target.hpBar = current; // 몬스터한테도 체력바 알려줌
                current.ownerMonster = target; // 주인지정
                current.gameObject.SetActive(true);
                return;
            };
        };

    }
    //////////////////////////////////////////////////////////////////////////////////////////////////
}
