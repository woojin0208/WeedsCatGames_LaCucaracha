using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// 플레이어 상태 전이와 입력 처리를 제어한다.
public class PlayerController : StateMachine<PlayerController>, IStatusEffectHandler
{

    [SerializeField] private KeyBindingData keyBindingData;
    [SerializeField] private Transform textPosition;
    public MovementAPI Move { get; private set; }
    public PlayerAnimationAPI Anim { get; private set; }
    public GameplayInputState Input { get; private set; }

    private WeaponBase? currentWeapon;

    public PlayerMovement playerMovement { get; private set; }

    protected PlayerRenderer playerRenderer;
    private PlayerInteraction playerInteraction;
    private PlayerBase playerBase;
    private PlayerInventory playerInventory;
    private bool canThrow = true;
    private bool isStop;

    private Coroutine slowEffectCoroutine;
    private void Awake()
    {
        var movement = GetComponent<PlayerMovement>();
        Move = new MovementAPI(movement, movement as IDashable);
        Anim = new PlayerAnimationAPI(GetComponentInChildren<PlayerRenderer>());
        Input = new GameplayInputState(keyBindingData);

        playerMovement = GetComponent<PlayerMovement>();
        playerInteraction = GetComponentInChildren<PlayerInteraction>();
        playerBase = GetComponent<PlayerBase>();
        playerInventory = GetComponentInChildren<PlayerInventory>();
    }

    private void Start()
    {
        // 씬 전환 이후 현재 장착 무기를 다시 연결한다.
        PlayerManager.Instance.Init(this);

        PlayerManager.Instance.PlayerTextPosition = textPosition;

        GameManager.Instance.GameEventAction += HandleGameEvent;
    }

    protected override void Update()
    {
        if (Time.timeScale == 0) return;
        if (CheckStateInput())
            return;
        base.Update();

        if (Input.InteractPressed) playerInteraction.TryInteraction();

        if (Input.ThrowPressed) TryThrow(Input.MouseWorldPosition);

        int selectWeaponNum = Input.GetSelectWeaponNumber();
        if (selectWeaponNum >= 0) playerInventory.ChangeWeapon(selectWeaponNum);

        Debug.Log(currentState);
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
            // 대시는 수평 입력과 무관하게 먼저 검사한다.
            if (state.CanDash && Input.DashPressed)
            {
                ChangeState(new PlayerDashState());
                return true;
            }

            // 이동 가능한 상태에서는 수평 입력이 있으면 걷기 상태로 전환한다.
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

        currentWeapon = weapon;

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
                if (currentState is IPlayerState state)
                {
                    if (state.CanClingWall)
                        ChangeState(new PlayerWallClingState(effectData));
                }
                break;
            case EffectKind.Slow:
                playerBase.slowCoroutine = StartCoroutine(playerBase.OnSlowEffect(effectData.duration, effectData.rate));
                break;
            case EffectKind.Damage:
                break;

        }
    }

    public void IgnoreEffect(StatusEffectData effectData)
    {
        if (!effectData.target.Contains(EffectTargetKind.Player)) return;

        switch (effectData.kind)
        {
            case EffectKind.WallJump:
                ChangeState(new PlayerFallState());
                break;
            case EffectKind.Slow:
                StopCoroutine(playerBase.slowCoroutine);
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

    public bool TryPipeWarp(bool isStart, PipeEnterance pipe)
    {
        if (currentState is IPlayerState state)
        {
            if (state.CanPipeWarp)
            {
                Vector3 targetPos = pipe.transform.position;

                // 파이프 시작 위치에 맞춰 X축 보정값을 적용한다.
                targetPos.x += -pipe.XOffset;

                ChangeState(new PlayerPipeWarpState(isStart, pipe.IsLeftStart));

                transform.position = targetPos;
                return true;
            }
            
        }

        return false;
    }
}