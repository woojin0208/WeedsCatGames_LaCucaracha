using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    /*
    [SerializeField]
    private KeyBindingData keyBindingData;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private PlayerController playerController;
    private PlayerInteraction playerInteraction;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponentInChildren<PlayerAttack>();
        playerController = GetComponent<PlayerController>();
        playerInteraction = GetComponentInChildren<PlayerInteraction>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return; // 설정 창 또는 Pause 상태일시 입력 금지

        // x = Horizontal Value
        float x = Input.GetKey(keyBindingData.keys[(int)KeyType.Left]) ? -1 : Input.GetKey(keyBindingData.keys[(int)KeyType.Right]) ? 1 : 0;
        //float y = Input.GetKey(keyBindingData.keys[(int)KeyType.Up]) ? -1 : Input.GetKey(keyBindingData.keys[(int)KeyType.Down]) ? 1 : 0;
        // Dash 키 입력시 true를, 아닐 시 false 를 인자로 보냄.
        //playerMovement.SetIsDash(Input.GetKey(keyBindingData.keys[(int)KeyType.Dash]) ? true : false);

        playerMovement.OnMovement(x, false);
        //playerMovement.TryLadderMove(y);
        if (Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Jump]))
        {
            playerMovement.OnJump();
        }

        if (Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Dash]))
        {
            playerMovement.TryDash();
        }

        if (Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Attack]))
        {
            playerAttack.Attack();
        }

        if (Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Interaction])) playerInteraction.TryInteraction();

        if (Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Throw]))
        {
            Vector3 throwPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            throwPosition.z = 0;
            //Debug.Log(throwPosition);
            playerController.TryThrow(throwPosition);
        }

        /*
        if (Input.GetKeyDown(KeyCode.M))
        {
            UIManager.Instance.OpenMinimap();
        }
    }
        */
    
}
