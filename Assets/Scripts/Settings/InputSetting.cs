using TMPro;
using UnityEngine;

public class InputSetting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] changeKeyName;
    private KeyBindingData keyBindingData;

    private readonly bool[] isClickKeyChanger = new bool[(int)KeyType.KeyCount];
    private bool isPauseInputSubscribed;

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
        SetInputEventSubscription(true);
        if (!TryEnsureKeyBindingData()) return;

        int labelCount = Mathf.Min(changeKeyName.Length, (int)KeyType.KeyCount);
        for (int i = 0; i < labelCount; i++)
        {
            changeKeyName[i].text = keyBindingData.GetGameplayBinding((KeyType)i).Primary.ToString();
        }
    }

    private void OnDisable()
    {
        SetInputEventSubscription(false);
    }

    private void OnGUI()
    {
        Event currentEvent = Event.current;
        if (currentEvent.keyCode == KeyCode.Escape) return;

        for (int i = 0; i < isClickKeyChanger.Length; i++)
        {
            if (!isClickKeyChanger[i]) continue;

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

    private void SetInputEventSubscription(bool isSubscribe)
    {
        InputStateManager manager = InputStateManager.Instance;
        if (manager == null) return;

        if (isSubscribe)
        {
            manager.PauseToggleRequested -= HandlePauseToggleRequested;
            manager.PauseToggleRequested += HandlePauseToggleRequested;
            isPauseInputSubscribed = true;
        }
        else
        {
            manager.PauseToggleRequested -= HandlePauseToggleRequested;
            isPauseInputSubscribed = false;
        }
    }

    private void HandlePauseToggleRequested()
    {
        if (!isPauseInputSubscribed || !gameObject.activeInHierarchy) return;
        ExitInputSetting();
    }

    public void ExitInputSetting()
    {
        UIManager.Instance.OnPausePanel();
        gameObject.SetActive(false);
    }

    private void ChangeKeyInput(int keyTypeIndex, KeyCode pressedKeyCode)
    {
        if (pressedKeyCode == KeyCode.None) return;
        if (!TryEnsureKeyBindingData()) return;

        KeyType selectedType = (KeyType)keyTypeIndex;

        for (int i = 0; i < (int)KeyType.KeyCount; i++)
        {
            KeyType compareType = (KeyType)i;
            if (compareType == selectedType) continue;

            if (keyBindingData.GetGameplayBinding(compareType).Primary != pressedKeyCode) continue;

            KeyCode previousKey = keyBindingData.GetGameplayBinding(selectedType).Primary;
            keyBindingData.SetGameplayKey(selectedType, pressedKeyCode);
            keyBindingData.SetGameplayKey(compareType, previousKey);

            if (keyTypeIndex < changeKeyName.Length) changeKeyName[keyTypeIndex].text = pressedKeyCode.ToString();
            if (i < changeKeyName.Length) changeKeyName[i].text = previousKey.ToString();

            isClickKeyChanger[keyTypeIndex] = false;
            return;
        }

        keyBindingData.SetGameplayKey(selectedType, pressedKeyCode);
        if (keyTypeIndex < changeKeyName.Length) changeKeyName[keyTypeIndex].text = pressedKeyCode.ToString();
        isClickKeyChanger[keyTypeIndex] = false;
    }

    private bool TryEnsureKeyBindingData()
    {
        if (keyBindingData != null) return true;

        InputStateManager manager = InputStateManager.Instance;
        if (manager == null || manager.BindingData == null)
        {
            Debug.LogError("InputSetting requires InputStateManager with KeyBindingData.");
            return false;
        }

        keyBindingData = manager.BindingData;
        return true;
    }

    public void ClickChangeKey(int i)
    {
        for (int j = 0; j < isClickKeyChanger.Length; j++)
        {
            isClickKeyChanger[j] = false;
        }

        if (i >= 0 && i < isClickKeyChanger.Length)
        {
            isClickKeyChanger[i] = true;
        }
    }
}
