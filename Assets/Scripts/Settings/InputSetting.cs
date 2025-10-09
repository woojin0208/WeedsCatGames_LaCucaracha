using TMPro;
using UnityEngine;

public enum KeyType
{
    Attack = 0, Jump = 1, Dash = 2, Up = 3, Down = 4, Left = 5, Right = 6,
    Throw = 7, Interaction = 8, KeyCount = 9
};
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

        for (int i = 0; i < isClickKeyChanger.Length; i++) // 키 변경이 활성화 되어있을 시
        {
            if (isClickKeyChanger[i])
            {
                if (currentEvent.type == EventType.KeyDown)
                {
                    ChangeKeyInput(i, currentEvent.keyCode); // 변경할 키와 현재 누른 KeyCode 를 인자로 보냄 
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
    private void ChangeKeyInput(int keyType, KeyCode pressedKeyCode) // 변경 키가 눌릴 시 , 해당 키 text 변경 및 키 변경 함수 호출
    {
        if (pressedKeyCode != KeyCode.None)
        {
            for (int i = 0; i < keyBindingData.keys.Length; i++)
            {
                if (keyBindingData.keys[i] == pressedKeyCode && (keyType != i))// 이미 사용 중인 keyCode를 지정한 경우
                {
                    // 서로의 keyCode를 변경
                    KeyCode tempKey = keyBindingData.keys[keyType];
                    //keyBindingData.keys[(int)keyType] = pressedKeyCode;
                    //keyBindingData.keys[i] = tempKey;
                    GameManager.Instance.KeyChanger((KeyType)keyType, pressedKeyCode);
                    GameManager.Instance.KeyChanger((KeyType)i, tempKey);
                    changeKeyName[keyType].text = pressedKeyCode.ToString();
                    changeKeyName[i].text = tempKey.ToString();
                    return;
                }
            }
            GameManager.Instance.KeyChanger((KeyType)keyType, pressedKeyCode); // 실제로 키를 변경하는 함수
            changeKeyName[keyType].text = pressedKeyCode.ToString();
            isClickKeyChanger[keyType] = false;

        }
    }

    public void ClickChangeKey(int i) // 키 변경 버튼을 누를 시 실행되는 함수. 
    {
        for (int j = 0; j < isClickKeyChanger.Length; j++)
        {
            isClickKeyChanger[j] = false;
        }
        isClickKeyChanger[i] = true; // 변경할 값에 해당하는 key의 값 확인
    }
}
