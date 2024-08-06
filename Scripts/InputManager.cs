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
    /// <summary> �� ���� </summary>
    Off,
    /// <summary> ��� ���� </summary>
    Down,
    /// <summary> ���� </summary>
    On,
    /// <summary> ��� �� </summary>
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

    //               ex) mouseState[��Ŭ��] = On, off, ...�̵� �� ���콺 Ű���� ���¸� ��������
    private static KeyState[] mouseState = new KeyState[(int)MouseCode.Length]; 
    
    public static GameObject mouseHitInterface { get; protected set; }  // ���콺�� �ö��ִ� UI
    public GraphicRaycaster uiCaster;                                   // UI��� ����ĳ����

    public static Transform mouseHitTransform { get; protected set; }   // ���콺�� �ε��� �ְ� �������
    public static Vector3 mouseHitPosition { get; protected set; }      // ���콺�� �ε��� ��ġ�� �������
    public static Vector3 mouseWorldPosition { get; protected set; }    // ���콺�� ���� ���� ��� �ִ���
    public static Vector2 mouseChangedPosition { get; protected set; }  // ���콺�� ��ġ ��ȭ��
    public static Vector3 mouseLastPosition { get; protected set;}      // ���콺�� ������ ��ġ -> lastUpdate���� ��곡���� ����

    public static Vector3 lastWantMoveDirection { get; protected set; } // ���������� �������� ����(�������� �ʾƵ� �Է� ��������)
    public static Vector3 moveDirection { get; protected set; }         // �����̴� ����
    public static float moveMagnitude { get; protected set; }           // �����̴� ��

    public static float mouseSensitive = 4.0f;                          // ���콺 �ΰ���

    void Start()
    {
        this.Singleton(ref _instance);
        // ����ĳ���� ã�ƿ���
        if (uiCaster == null)
            uiCaster = FindObjectOfType<GraphicRaycaster>();
    }

    void Update()
    {
        #region MovementInput
        Vector3 currentMove = Vector3.zero; // �̷��� �����δ�

        // ����Ű�� �Է¹޽��ϴ�
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) currentMove += Vector3.left;
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) currentMove += Vector3.down;
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) currentMove += Vector3.right;
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) currentMove += Vector3.up;

        // �׸��� �̰��� ũ�� 1�� ������ �밢�� �������� �� �������� �ʵ��� ����
        currentMove.Normalize();

        // �̵��� 0�̶�� �ȿ����̵����Ѵ�
        if(currentMove.magnitude <= 0)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            moveDirection = currentMove;
            lastWantMoveDirection = currentMove;
        };

        //�������� ũ�� ����
        moveMagnitude = currentMove.magnitude;

        // �ӵ��� �ʹ� ������    �ӵ����� 
        if (moveMagnitude > 1) moveDirection = currentMove.normalized;
        // ??
        else moveDirection = currentMove;
        #endregion

        #region MouseInput
        // ���콺�� �󸶳� ���������� üũ
        mouseChangedPosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // ���콺�� UI�� ��ġ���� Ȯ��
        if(uiCaster)
        {
            var data = new PointerEventData(null); 
            data.position = Input.mousePosition; // ���콺�� ��ġ

            List<RaycastResult> resultList = new List<RaycastResult>(); //UI�� ����ĳ��Ʈ�� ����� �޴� �ڷ����̴�
            uiCaster.Raycast(data, resultList);

            if (resultList.Count > 0) mouseHitInterface = resultList[0].gameObject; // ���콺�� �ö� ���� UI�� 1���̻� �ִٸ� �����ǰ� �޾ƿ�
            else mouseHitInterface = null;
        }

        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //��ũ�� ��ġ�� ������ ������ش�
        Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit3D;

        Physics.Raycast(currentRay, out hit3D);

        if(hit3D.collider != null) // ������ �ִٸ�
        {
            mouseHitPosition = hit3D.point;
            mouseHitTransform = hit3D.transform;
        }
        else
        {
            mouseHitPosition = Vector3.zero;
        }

        // ���콺 ��ư�� ���� ������Ʈ
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
        // �ٸ� ���콺 �Էµ��� ������ ����
        mouseLastPosition = Input.mousePosition; 
    }

    public static KeyState GetKeyState(MouseCode target)
    {
        return mouseState[(int)target];
    }
}
