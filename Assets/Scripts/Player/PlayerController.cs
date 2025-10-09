using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 1: Idle, 2: Walk, 3: Dash, 4: Jump, 5: Attack
/// </summary>
public enum PlayerState { Idle, Walk, Dash, Jump, Attack, ClingWall, OnLadder }
public class PlayerController : StateMachine<PlayerController>, IStatusEffectHandler
{

    [SerializeField] private KeyBindingData keyBindingData;
    [SerializeField] private Transform textPosition;
    public MovementAPI Move { get; private set; }
    public PlayerAnimationAPI Anim {  get; private set; }
    public InputAPI Input { get; private set; }
    //public LadderAPI Ladder { get; private set; }
    //public WallClingAPI WallCling { get; private set; }
    //public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    private WeaponBase? currentWeapon;

    public  PlayerMovement playerMovement { get; private set; }

    protected PlayerRenderer playerRenderer;
    private PlayerInteraction playerInteraction;
    private PlayerBase playerBase;
    private PlayerInventory playerInventory;
    private bool canThrow = true; // Debug용 
    private bool isStop;
    //private bool canClimbLadder = false;
    private void Awake()
    {
        // API initialization
        var movement = GetComponent<PlayerMovement>();
        Move = new MovementAPI(movement, movement as IDashable);
        Anim = new PlayerAnimationAPI(GetComponentInChildren<PlayerRenderer>());
        Input = new InputAPI(keyBindingData);

        playerMovement = GetComponent<PlayerMovement>();
        playerInteraction = GetComponentInChildren<PlayerInteraction>();
        playerBase = GetComponent<PlayerBase>();
        playerInventory = GetComponentInChildren<PlayerInventory>();
        playerBase.OnHitAction += HandleOnHit;
    }

    private void Start()
    {
        // Scene 변경 시 현재 무기 할당
        PlayerManager.Instance.Init(this);

        PlayerManager.Instance.PlayerTextPosition = textPosition;

        ChangeState(new PlayerIdleState());

        GameManager.Instance.GameEventAction += HandleGameEvent;
    }

    protected override void Update()
    {
        if (Time.timeScale == 0) return;
        //Debug.Log("Player State : " + currentState);
        if (CheckStateInput())
            return;
        base.Update();

        if (Input.InteractPressed) playerInteraction.TryInteraction();

        if (Input.ThrowPressed) TryThrow(Input.MouseWorldPosition);

        int selectWeaponNum = Input.GetSelectWeaponNumber();
        if (selectWeaponNum >= 0) playerInventory.ChangeWeapon(selectWeaponNum);
    }

    private void HandleOnHit()
    {
        Anim.TakeDamaged();
    }

    public bool IsGrounded => playerMovement.IsGrounded;

    private void HandleGameEvent(bool isStop)
    {
        this.isStop = isStop;
    }
    private bool CheckStateInput()
    {
        if (isStop) return false;

        if (currentState is IPlayerState state)
        {
            float h = Input.Horizontal;
            // 1) 우선 dash 검사 (horizontal 여부와 상관없이)
            //Debug.Log($"isPress : {Input.DashPressed} / canDash : {state.CanDash} / horizontal : {h} ");
            if (state.CanDash && Input.DashPressed)
            {
                ChangeState(new PlayerDashState());
                return true;
            }

            // 2) 그 다음에 걷기 검사
            if (state.CanWalk && Mathf.Abs(h) > 0.01f)
            {
                ChangeState(new PlayerWalkState());
                return true;
            }

            if (state.CanJump)
            {
                if (Input.JumpPressed && Move.RemainingJumps > 0)
                {
                    Debug.Log("점프!!!!!!!!!!!!!!!!!!");
                    ChangeState(new PlayerJumpState());
                    return true;
                }
            }

            if (state.CanAttack)
            {
                if (Input.AttackPressed)
                {
                    ChangeState(new PlayerAttackState());
                    return true;
                }
            }
        }
        return false;
    }

    public void GetWeapon(WeaponBase weapon)
    {
        PlayerManager.Instance.SetWeapon(weapon);

        currentWeapon = weapon;// == null ? null : weapon;
        //if (weapon == null) return;

        Debug.Log($"CurrentWeapon == {currentWeapon}, pm.CW == {PlayerManager.Instance.CurrentWeapon}, pm.CI = {PlayerManager.Instance.CurrentEquipId}");
        weapon.gameObject.transform.parent = playerInventory.transform;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localScale = transform.localScale;

    }
    public void TryThrow(Vector2 throwPosition)
    {
        if (PlayerManager.Instance.CurrentWeapon == null)
        {
            return;
        }
        canThrow = false;
        Anim.Throw();
        StartCoroutine(DelayedThrow(throwPosition, 0.24f));
    }

    public void RemoveWeapon()
    {
        currentWeapon.gameObject.SetActive(false);
    }
    private IEnumerator DelayedThrow(Vector2 throwPosition, float elapsedTime = 0)
    {
        yield return new WaitForSeconds(elapsedTime);
        Debug.Log(currentWeapon);
        currentWeapon.OnThrow(throwPosition);
        canThrow = true;
    }

    public void ApplyEffect(StatusEffectData effectData)
    {
        if (!effectData.target.Contains(EffectTargetKind.Player)) return;

        switch (effectData.kind)
        {
            case EffectKind.WallJump:
                
                ChangeState(new PlayerWallClingState(effectData.duration));
                break;
            case EffectKind.Slow:
                break;
            case EffectKind.Damage:
                break;

        }
    }

    public void OnLadder(Vector2 startPosition, Vector2 endPosition)
    {
        if (currentState is IPlayerState state)
        {
            if (state.CanLadder)
                ChangeState(new PlayerClimbLadderState(startPosition, endPosition));
        }
    }
}
