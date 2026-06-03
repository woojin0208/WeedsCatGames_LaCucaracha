using UnityEngine;

// 무기의 공통 상호작용, 장착, 투척, 내구도 처리를 담당한다.
public class WeaponBase : MonoBehaviour, IInteractable
{
    private const int DurabilityUseAmount = 1;
    private const float EnemyThrowDamageMultiplier = 2f;

    [field: SerializeField] public WeaponDefinition WeaponDefinition { get; private set; }

    [Header("Weapon Stats")]
    [SerializeField] private float weaponDamage;
    [SerializeField] private float throwSpeed;

    public float Damage => weaponDamage;

    [field: SerializeField] public int Durability { get; private set; } = 1;
    [field: SerializeField] public Sprite WeaponSprite { get; private set; }
    [field: SerializeField] public Transform InteractivePos { get; set; }

    [SerializeField] private Vector2 weaponPosition;

    [Header("Runtime References")]
    [SerializeField] private EntityBase entityBase;
    [SerializeField] private Collider2D trigger2D;
    [SerializeField] private Collider2D collider2D;

    [field: SerializeField] public string InstanceId { get; private set; }

    private Rigidbody2D rigidbody2D;
    private WeaponRenderer weaponRenderer;

    private bool isThrown;
    private Vector2 throwDirection;
    private bool isDestroyed;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        weaponRenderer = GetComponent<WeaponRenderer>();
    }

    private void Start()
    {
        if (transform.parent != null)
        {
            entityBase = GetComponentInParent<EntityBase>();
            GetWeapon();
        }
        else
        {
            EnableWorldInteraction();
        }

        SetPlayerWeaponCollisionIgnored(true);
        RemoveCloneNameSuffix();
    }

    private void Update()
    {
        if (entityBase == null || isThrown) return;

        transform.localPosition = weaponPosition;

        if (rigidbody2D != null)
        {
            rigidbody2D.velocity = Vector2.zero;
        }
    }

    public void BindInstance(string id, int durability)
    {
        InstanceId = id;
        Durability = durability;
    }

    public EntityBase GetEntity()
    {
        return entityBase;
    }

    public void GetWeapon()
    {
        transform.localPosition = weaponPosition;
    }

    public void OnThrow(Vector2 targetPosition)
    {
        if (rigidbody2D == null) return;

        if (entityBase == null)
        {
            entityBase = GetComponentInParent<EntityBase>();
        }

        DetachFromOwner();
        ApplyThrowVelocity(targetPosition);
        EnableThrowCollision();
        RemoveFromInventory();
    }

    public void Interactive(PlayerBase player)
    {
        PlayerManager playerManager = PlayerManager.Instance;
        if (playerManager == null) return;

        // 변경점: 인벤토리가 가득 찼다면 무기 상태를 바꾸지 않고 종료한다.
        if (!playerManager.HasEmptyWeaponSlot())
        {
            Debug.LogWarning("[WeaponBase] 인벤토리가 가득 차 무기를 획득할 수 없습니다.", this);
            return;
        }

        WeaponInstance instance = playerManager.AddWeapon(this);
        if (instance == null) return;

        entityBase = player;
        gameObject.SetActive(false);
    }

    private void DetachFromOwner()
    {
        transform.parent = null;
        entityBase = null;
        isThrown = true;

        PlayerManager.Instance?.SetWeapon(null);
    }

    private void ApplyThrowVelocity(Vector2 targetPosition)
    {
        rigidbody2D.gravityScale = 0f;

        throwDirection = (targetPosition - (Vector2)transform.position).normalized;
        rigidbody2D.velocity = throwDirection * throwSpeed;
    }

    private void EnableThrowCollision()
    {
        if (trigger2D != null) trigger2D.enabled = true;
        if (collider2D != null) collider2D.enabled = false;

        SetPlayerWeaponCollisionIgnored(false);
    }

    private void EnableWorldInteraction()
    {
        if (trigger2D != null) trigger2D.enabled = true;
    }

    private void RemoveFromInventory()
    {
        if (string.IsNullOrWhiteSpace(InstanceId)) return;

        PlayerManager.Instance?.RemoveWeapon(InstanceId);
    }

    private bool UseDurability()
    {
        Durability -= DurabilityUseAmount;
        return Durability <= 0;
    }

    private void DestroyWeapon(EffectTargetKind target)
    {
        if (isDestroyed) return;

        isDestroyed = true;
        isThrown = false;

        PlayerManager.Instance?.SetWeapon(null);
        SetPlayerWeaponCollisionIgnored(true);
        if (trigger2D != null) trigger2D.enabled = false;
        if (collider2D != null) collider2D.enabled = false;

        if (rigidbody2D != null)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.angularVelocity = 0f;
            rigidbody2D.gravityScale = 0f;
        }

        if (TryGetComponent<EffectableWeapon>(out EffectableWeapon effectable))
        {
            var context = new WeaponEffectContext(target, throwDirection, transform.position);
            effectable.OnDestruction(context);
        }

        if (weaponRenderer != null)
        {
            weaponRenderer.OnDestruction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isThrown) return;

        if (collision.CompareTag(GameTags.Enemy))
        {
            HitEnemy(collision);
            return;
        }

        if (collision.CompareTag(GameTags.Ground))
        {
            HitSurface(EffectTargetKind.Ground);
            return;
        }

        if (collision.CompareTag(GameTags.Wall))
        {
            HitSurface(EffectTargetKind.Wall);
        }
    }

    private void HitEnemy(Collider2D collision)
    {
        EntityBase entity = collision.GetComponent<EntityBase>();
        if (entity != null)
        {
            entity.TakeDamage(weaponDamage * EnemyThrowDamageMultiplier);
        }

        if (UseDurability())
        {
            DestroyWeapon(EffectTargetKind.Enemy);
            return;
        }

        StopThrow();
    }

    private void HitSurface(EffectTargetKind target)
    {
        if (UseDurability())
        {
            DestroyWeapon(target);
            return;
        }

        StopThrow();
    }

    private void StopThrow()
    {
        isThrown = false;

        SetPlayerWeaponCollisionIgnored(true);

        if (rigidbody2D != null)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.gravityScale = 1f;
        }

        if (collider2D != null)
        {
            collider2D.enabled = true;
        }
    }

    private void SetPlayerWeaponCollisionIgnored(bool isIgnored)
    {
        int playerLayer = GameLayers.PlayerIndex;
        int weaponLayer = GameLayers.WeaponIndex;

        if (playerLayer < 0 || weaponLayer < 0)
        {
            Debug.LogWarning("[WeaponBase] Player 또는 Weapon Layer를 찾을 수 없습니다.", this);
            return;
        }

        Physics2D.IgnoreLayerCollision(playerLayer, weaponLayer, isIgnored);
    }

    private void RemoveCloneNameSuffix()
    {
        const string cloneSuffix = "(Clone)";

        if (!gameObject.name.Contains(cloneSuffix)) return;

        gameObject.name = gameObject.name.Replace(cloneSuffix, string.Empty).Trim();
    }
}