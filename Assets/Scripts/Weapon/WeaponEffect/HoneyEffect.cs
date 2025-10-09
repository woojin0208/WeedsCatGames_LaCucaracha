using UnityEngine;

public class HoneyEffect : WeaponEffectBase
{
    private bool isEnemyTarget = false;
    private EffectBase effectBase;

    private void Awake()
    {
        effectBase = GetComponent<EffectBase>();
    }
    private void Update()
    {
        if (!isEnemyTarget) return;

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        if (rayHit.collider != null)
        {
            float targetY = rayHit.point.y;
            /*
            if (transform.position.y > targetY)
            {
                float newY = Mathf.MoveTowards(transform.position.y, targetY, 0.4f * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
            */
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        }
    }
    public override void Initialize(EffectTargetKind target)
    {
        base.Initialize(target);
        Debug.Log(target);
        if (target == EffectTargetKind.Wall)
        {
            //Debug.Log("È¸Àü");
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (target == EffectTargetKind.Enemy)
        {
            isEnemyTarget = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (target == EffectTargetKind.Ground)
        {

        }
    }

}
