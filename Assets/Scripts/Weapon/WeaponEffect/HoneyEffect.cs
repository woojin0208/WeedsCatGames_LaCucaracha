using UnityEngine;

public class HoneyEffect : WeaponEffectBase
{
    private bool isEnemyTarget = false;

    private void Update()
    {
        if (!isEnemyTarget) return;

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        if (rayHit.collider != null)
        {
            float targetY = rayHit.point.y;

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
