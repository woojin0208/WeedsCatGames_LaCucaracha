using UnityEngine;

// 무기의 공통 동작과 상태를 정의한다.
public class WeaponBase : MonoBehaviour, IWeaponable, IInteractable
{
    [field: SerializeField] public WeaponDefinition WeaponDefinition { get; private set; }

    [Header("Weapon Stats")]
    [SerializeField] private float weaponDamage;
    [SerializeField] private float throwSpeed;

    [field: SerializeField] public int Durability { get; private set; } = 1;
    [field: SerializeField] public Sprite WeaponSprite { get; private set; }
    [field: SerializeField] public Transform InteractivePos { get; set; }

    [SerializeField] Vector2 weaponPosition;

    private PlayerController playerController;
    private PlayerInventory inventory;
    private WeaponRenderer weaponRenderer;
    [SerializeField]
    private EntityBase entityBase;

    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private Collider2D trigger2D;
    [SerializeField]
    private Collider2D collider2D;
    private Vector2 throwPosition = Vector2.zero;

    private bool onAttack = false;
    private bool onThrow = false;
    [field: SerializeField] public string InstanceId { get; private set; }

    private bool isLeftThrow;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        weaponRenderer = GetComponent<WeaponRenderer>();
    }

    private void Start()
    {
        if (transform.parent != null)
        {
            playerController = GetComponentInParent<PlayerController>();
            entityBase = GetComponentInParent<EntityBase>();
            GetWeapon();
        }
        else
        {
            trigger2D.enabled = true;
        }

        Physics2D.IgnoreLayerCollision(6, 8, true);

        if (gameObject.name.Contains("(Clone)"))
            gameObject.name = gameObject.name.Replace("(Clone)", "").Trim();
    }

    private void Update()
    {
        if (entityBase != null)
        {
            if (!onThrow)
            {
                transform.localPosition = weaponPosition;
                rigidbody2D.velocity = Vector2.zero;
            }
        }
    }

    public void BindInstance(string id) => InstanceId = id;

    public virtual void GetWeapon()
    {
        transform.localPosition = weaponPosition;
    }

    // 월드에서 사라질 때 무기 오브젝트를 비활성화한다.
    public virtual void PutWeapon()
    {
        gameObject.SetActive(false);
    }

    // 공격 시 무기별 동작을 실행한다.
    public virtual void OnAttack()
    {
    }

    // 무기 소유자가 던지기를 실행했을 때 물리 이동을 시작한다.
    public virtual void OnThrow(Vector2 throwPosition)
    {
        if (entityBase == null) entityBase = GetComponentInParent<EntityBase>();

        transform.parent = null;
        this.throwPosition = throwPosition;

        onThrow = true;
        rigidbody2D.gravityScale = 0;
        entityBase = null;
        Vector2 direction = (throwPosition - (Vector2)transform.position).normalized;

        rigidbody2D.velocity = direction * throwSpeed;

        isLeftThrow = rigidbody2D.velocity.x > 0 ? false : true;
        PlayerManager.Instance.SetWeapon(null);

        trigger2D.enabled = true;
        collider2D.enabled = false;
        Physics2D.IgnoreLayerCollision(6, 8, false);

        // 투척 직후 인벤토리 목록에서도 해당 무기를 제거한다.
        PlayerManager.Instance.RemoveWeapon(InstanceId);
    }

    // 플레이어가 월드 무기와 상호작용했을 때 인벤토리에 추가한다.
    public virtual void Interactive(PlayerBase player)
    {
        if (entityBase == null)
        {
            entityBase = player;
            playerController = player.GetComponent<PlayerController>();
            inventory = playerController.GetComponentInChildren<PlayerInventory>();
        }

        var pm = PlayerManager.Instance;

        // 먼저 인벤토리 데이터에 등록한 뒤 월드 오브젝트를 숨긴다.
        var inst = pm.AddWeapon(gameObject.name, Durability);
        if (inst == null) return;

        gameObject.SetActive(false);
    }

    private bool CheckDurability()
    {
        int useDurCount = 1;
        Durability -= useDurCount;

        return Durability <= 0 ? true : false;
    }

    private void DestructionWeapon(EffectTargetKind target)
    {
        PlayerManager.Instance.SetWeapon(null);

        if (TryGetComponent<EffectableWeapon>(out var effectable))
        {
            effectable.OnDestruction(target, isLeftThrow);
        }
        rigidbody2D.velocity = Vector2.zero;

        weaponRenderer.OnDestruction();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!onThrow) return;

        if (collision.CompareTag("Enemy"))
        {
            EntityBase entity = collision.GetComponent<EntityBase>();
            if (entity != null) entity.TakeDamage(weaponDamage * 2);
            onThrow = false;
            if (CheckDurability()) DestructionWeapon(EffectTargetKind.Enemy);
            rigidbody2D.velocity = Vector2.zero;
        }
        else if (collision.CompareTag("Ground"))
        {
            if (CheckDurability()) DestructionWeapon(EffectTargetKind.Ground);
        }
        else if (collision.CompareTag("Wall"))
        {
            if (CheckDurability()) DestructionWeapon(EffectTargetKind.Wall);
        }

        onThrow = false;
        Physics2D.IgnoreLayerCollision(6, 8, true);
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.gravityScale = 1f;
        Debug.Log(collision.gameObject.name);
        collider2D.enabled = true;
    }

    public EntityBase GetEntity()
    {
        return entityBase;
    }
}