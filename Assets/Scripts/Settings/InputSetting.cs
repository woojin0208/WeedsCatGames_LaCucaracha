using TMPro;
using UnityEngine;

// 프로젝트에서 사용하는 입력 키 종류를 정의한다.
public enum KeyType
{
    Attack = 0, Jump = 1, Dash = 2, Up = 3, Down = 4, Left = 5, Right = 6,
    Throw = 7, Interaction = 8, KeyCount = 9
};

// 키 설정 UI와 입력 변경 처리를 담당한다.
public class InputSetting : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] changeKeyName;
    [SerializeField]
    private KeyBindingData keyBindingData;
    private bool[] isClickKeyChanger = new bool[(int)KeyType.KeyCount];

    private void Awake()
    {
        for (int i = 0; i < isClickKeyChanger.Length; i++)
        {
            isClickKeyChanger[i] = false;
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        for (int i = 0; i < changeKeyName.Length; i++)
        {
            changeKeyName[i].text = keyBindingData.keys[i].ToString();
        }
    }
    private void OnGUI()
    {
        Event currentEvent = Event.current;
        if (currentEvent.keyCode == KeyCode.Escape) return;

        // 키 변경 대기 중인 항목에만 새 입력을 반영한다.
        for (int i = 0; i < isClickKeyChanger.Length; i++)
        {
            if (isClickKeyChanger[i])
            {
                if (currentEvent.type == EventType.KeyDown)
                {
                    ChangeKeyInput(i, currentEvent.keyCode);
                }
                else if (currentEvent.type == EventType.MouseDown)
                {
                    KeyCode mouseKeyCode = KeyCode.None;
                    switch (currentEvent.button)
                    {
                        case 0: mouseKeyCode = KeyCode.Mouse0; break;
                        case 1: mouseKeyCode = KeyCode.Mouse1; break;
                    }
                    ChangeKeyInput(i, mouseKeyCode);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ExitInputSetting();
        }
    }

    public void ExitInputSetting()
    {
        UIManager.Instance.OnPausePanel();
        this.gameObject.SetActive(false);
    }

    // 선택한 키 슬롯에 새 입력을 반영하고 중복 키는 서로 교환한다.
    private void ChangeKeyInput(int keyType, KeyCode pressedKeyCode)
    {
        if (pressedKeyCode != KeyCode.None)
        {
            for (int i = 0; i < keyBindingData.keys.Length; i++)
            {
                if (keyBindingData.keys[i] == pressedKeyCode && (keyType != i))
                {
                    KeyCode tempKey = keyBindingData.keys[keyType];
                    GameManager.Instance.KeyChanger((KeyType)keyType, pressedKeyCode);
                    GameManager.Instance.KeyChanger((KeyType)i, tempKey);
                    changeKeyName[keyType].text = pressedKeyCode.ToString();
                    changeKeyName[i].text = tempKey.ToString();
                    return;
                }
            }
            GameManager.Instance.KeyChanger((KeyType)keyType, pressedKeyCode);
            changeKeyName[keyType].text = pressedKeyCode.ToString();
            isClickKeyChanger[keyType] = false;
        }
    }

    // 선택한 키 슬롯만 입력 대기 상태로 전환한다.
    public void ClickChangeKey(int i)
    {
        for (int j = 0; j < isClickKeyChanger.Length; j++)
        {
            isClickKeyChanger[j] = false;
        }
        isClickKeyChanger[i] = true;
    }
}