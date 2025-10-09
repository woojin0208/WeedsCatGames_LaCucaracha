using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputAPI
{
    private readonly KeyBindingData keyBindingData;

    public InputAPI(KeyBindingData keyBindingData)
    {
        this.keyBindingData = keyBindingData;
    }
    public float Horizontal =>
       Input.GetKey(keyBindingData.keys[(int)KeyType.Left]) ? -1 : Input.GetKey(keyBindingData.keys[(int)KeyType.Right]) ? 1 : 0;

    public bool JumpPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Jump]);
    public bool DashPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Dash]);
    public bool AttackPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Attack]);
    public bool InteractPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Interaction]);
    public bool ThrowPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Throw]);

    public bool SelectWeaponPressed => Input.GetKeyDown(KeyCode.Alpha1);

    public int GetSelectWeaponNumber()
    {
        int maxCount = PlayerManager.Instance.MaxWeaponCount;
        for (int i = 0; i < maxCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) return i;
        }

        return -1;
    }
    //public int WeaponPressed => Input.G

    public Vector2 MouseWorldPosition
    {
        get
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;
            return worldPos;
        }
    }


}
