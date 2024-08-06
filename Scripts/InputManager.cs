using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum MouseCode
{
    LeftClick,
    RightClick,
    WheelClick,
    ThumbDown,
    ThumbUp,

    Length
}

public enum KeyState
{
    /// <summary> 안 눌림 </summary>
    Off,
    /// <summary> 방금 눌림 </summary>
    Down,
    /// <summary> 눌림 </summary>
    On,
    /// <summary> 방금 뗌 </summary>
    Up,
}

public class InputManager : MonoBehaviour
{
    public static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameManager.Instance.gameObject.AddComponent<InputManager>();

            return _instance;
        }
    }

    //               ex) mouseState[좌클릭] = On, off, ...이됨 즉 마우스 키마다 상태를 가지도록
    private static KeyState[] mouseState = new KeyState[(int)MouseCode.Length]; 
    
    public static GameObject mouseHitInterface { get; protected set; }  // 마우스가 올라가있는 UI
    public GraphicRaycaster uiCaster;                                   // UI담당 레이캐스터

    public static Transform mouseHitTransform { get; protected set; }   // 마우스가 부딪힌 애가 어디인지
    public static Vector3 mouseHitPosition { get; protected set; }      // 마우스가 부딪힌 위치가 어디인지
    public static Vector3 mouseWorldPosition { get; protected set; }    // 마우스가 게임 세상 어디에 있는지
    public static Vector2 mouseChangedPosition { get; protected set; }  // 마우스의 위치 변화량
    public static Vector3 mouseLastPosition { get; protected set;}      // 마우스의 마지막 위치 -> lastUpdate에서 계산끝난후 저장

    public static Vector3 lastWantMoveDirection { get; protected set; } // 마지막으로 가고자한 방향(움직이지 않아도 입력 들어오도록)
    public static Vector3 moveDirection { get; protected set; }         // 움직이는 방향
    public static float moveMagnitude { get; protected set; }           // 움직이는 양

    public static float mouseSensitive = 4.0f;                          // 마우스 민감도

    void Start()
    {
        this.Singleton(ref _instance);
        // 레이캐스터 찾아오기
        if (uiCaster == null)
            uiCaster = FindObjectOfType<GraphicRaycaster>();
    }

    void Update()
    {
        #region MovementInput
        Vector3 currentMove = Vector3.zero; // 이렇게 움직인다

        // 방향키를 입력받습니다
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) currentMove += Vector3.left;
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) currentMove += Vector3.down;
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) currentMove += Vector3.right;
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) currentMove += Vector3.up;

        // 그리고 이것을 크기 1로 조정해 대각선 움직임이 더 빨라지지 않도록 조정
        currentMove.Normalize();

        // 이동이 0이라면 안움직이도록한다
        if(currentMove.magnitude <= 0)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            moveDirection = currentMove;
            lastWantMoveDirection = currentMove;
        };

        //움직임의 크기 갱신
        moveMagnitude = currentMove.magnitude;

        // 속도가 너무 빠르면    속도제한 
        if (moveMagnitude > 1) moveDirection = currentMove.normalized;
        // ??
        else moveDirection = currentMove;
        #endregion

        #region MouseInput
        // 마우스가 얼마나 움직였는지 체크
        mouseChangedPosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // 마우스가 UI에 걸치는지 확인
        if(uiCaster)
        {
            var data = new PointerEventData(null); 
            data.position = Input.mousePosition; // 마우스의 위치

            List<RaycastResult> resultList = new List<RaycastResult>(); //UI용 레이캐스트의 결과를 받는 자료형이다
            uiCaster.Raycast(data, resultList);

            if (resultList.Count > 0) mouseHitInterface = resultList[0].gameObject; // 마우스가 올라간 곳에 UI가 1개이상 있다면 맨위의거 받아옴
            else mouseHitInterface = null;
        }

        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //스크린 위치를 선으로 만들어준다
        Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit3D;

        Physics.Raycast(currentRay, out hit3D);

        if(hit3D.collider != null) // 맞은게 있다면
        {
            mouseHitPosition = hit3D.point;
            mouseHitTransform = hit3D.transform;
        }
        else
        {
            mouseHitPosition = Vector3.zero;
        }

        // 마우스 버튼의 상태 업데이트
        for(int i = 0; i < mouseState.Length; i++)
        {
            if (Input.GetMouseButtonDown(i)) mouseState[i] = KeyState.Down;
            else if (Input.GetMouseButtonUp(i)) mouseState[i] = KeyState.Up;
            else if (Input.GetMouseButton(i)) mouseState[i] = KeyState.On;
            else mouseState[i] = KeyState.Off;
        }

        #endregion
    }

    void LateUpdate()
    {
        // 다른 마우스 입력들이 끝나고 저장
        mouseLastPosition = Input.mousePosition; 
    }

    public static KeyState GetKeyState(MouseCode target)
    {
        return mouseState[(int)target];
    }
}
