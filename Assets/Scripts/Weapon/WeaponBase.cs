using UnityEngine;

public class WeaponBase : MonoBehaviour, IWeaponable, IInteractable
{
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
    private Collider2D trigger2D; // trigger collider
    [SerializeField]
    private Collider2D collider2D;
    private Vector2 throwPosition = Vector2.zero;

    private bool onAttack = false;
    private bool onThrow = false;
    [field: SerializeField] public string InstanceId { get; private set; }

    private void Awake()
    {
        //trigger2D = GetComponent<Collider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        weaponRenderer = GetComponent<WeaponRenderer>();
        //collider2D.enabled = true;

        //rigidbody2D.gravityScale = 0f;
        //rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
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
                //Physics2D.IgnoreLayerCollision(6, 8, true);
            }

        }

    }

    public void BindInstance(string id) => InstanceId = id;
    public virtual void GetWeapon()
    {
        /*
        this.transform.parent = playerController.GetComponentInChildren<PlayerRenderer>().gameObject.transform;
        this.transform.localScale = playerController.transform.localScale;
        this.transform.localPosition = weaponPosition;
        */

        /*
        if (inventory == null) inventory = playerController.GetComponentInChildren<PlayerInventory>();
        transform.parent = inventory.transform;
        this.transform.localScale = inventory.transform.localScale;
        this.transform.localPosition = weaponPosition;

        //inventory.AddWeapon(gameObject.name, Durability);

        entityBase.stats.GetStat(StatType.AttackDamage).BonusValue = weaponDamage;

        if (entityBase.CompareTag("Player"))
        {
            PlayerManager.Instance.SetWeapon(this);
        }
        */
        this.transform.localPosition = weaponPosition;

    }

    /// <summary>
    /// 무기를 내려놓을 시.
    /// 
    /// </summary>
    public virtual void PutWeapon()
    {
        /*
        this.transform.parent = null;
        entityBase = null;
        trigger2D.enabled = true;
        */
        gameObject.SetActive(false);

    }

    /// <summary>
    /// 공격 실행 시
    /// Animation 및 내구도 조정
    /// </summary>
    public virtual void OnAttack()
    {

    }

    /// <summary>
    /// 해당 무기를 소유한 Entity가 던지기 실행 시
    /// throwPosition까지 직선 이동. 이후 중력만큼 낙하.
    /// </summary>
    /// <param name="throwPosition"></param>
    public virtual void OnThrow(Vector2 throwPosition)
    {
        if (entityBase == null) entityBase = GetComponentInParent<EntityBase>();
        // 물리 연출
        this.transform.parent = null;
        this.throwPosition = throwPosition;

        onThrow = true;
        rigidbody2D.gravityScale = 0;
        //entityBase.stats.GetStat(StatType.AttackDamage).BonusValue -= weaponDamage;
        entityBase = null;
        Vector2 direction = (throwPosition - (Vector2)transform.position).normalized;

        rigidbody2D.velocity = direction * throwSpeed;

        PlayerManager.Instance.SetWeapon(null);

        //collider2D.enabled = false;
        trigger2D.enabled = true;
        collider2D.enabled = false;
        Physics2D.IgnoreLayerCollision(6, 8, false);

        // 리스트 정리
        PlayerManager.Instance.RemoveWeapon(InstanceId);
    }

    /// <summary>
    /// Entity 가 this object에 상호작용을 했을 때.
    /// </summary>
    /// <param name="entity"></param>
    public virtual void Interactive(EntityBase entity)
    {
        if (entityBase == null)
        {
            entityBase = entity;
            playerController = entity.GetComponent<PlayerController>();
            inventory = playerController.GetComponentInChildren<PlayerInventory>();
        }

        var pm = PlayerManager.Instance;

        // 데이터 먼저 추가
        var inst = pm.AddWeapon(gameObject.name, Durability);
        if (inst == null) return; // 꽉 참

        //BindInstance(InstanceId);
        //playerController = entity.GetComponent<PlayerController>();
        //pm.SetWeapon(this);
        //playerController.GetWeapon(this); // 부모/위치 설정은 여기서만
        //GetWeapon(); // 필요시 이펙트만
        gameObject.SetActive(false); // 혹은 풀/파괴

        /*
        // 장착은 항상 PM 경로로 → 여기서 Instantiate + BindInstance 수행
        pm.GetWeapon(inst);

        // 월드 오브젝트는 정리(비활성/파괴/풀 복귀)
        gameObject.SetActive(false);
        */
    }

    private bool CheckDurability()
    {
        //Debug.Log(durability);
        int useDurCount = 1;
        Durability -= useDurCount;

        //Debug.Log(durability);
        return Durability <= 0 ? true : false;
    }

    private void DestructionWeapon(EffectTargetKind target)
    {
        //if (entityBase != null)
        //    entityBase.stats.GetStat(StatType.AttackDamage).BonusValue -= weaponDamage;
        PlayerManager.Instance.SetWeapon(null);

        if (TryGetComponent<EffectableWeapon>(out var effectable))
        {
            effectable.OnDestruction(target);
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
