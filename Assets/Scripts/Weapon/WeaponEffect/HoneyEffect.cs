using UnityEngine;

// 꿀 효과의 생성 위치와 방향을 충돌 대상에 맞게 보정한다.
public class HoneyEffect : MonoBehaviour, IWeaponEffect
{
    public void InitializeFromWeaponHit(WeaponEffectContext context)
    {
        transform.position = context.HitPosition;

        switch (context.Target)
        {
            case EffectTargetKind.Wall:
                ApplyWallRotation(context.ThrowDirection);
                break;

            case EffectTargetKind.Enemy:
                SnapToGround();
                transform.rotation = Quaternion.identity;
                break;

            case EffectTargetKind.Ground:
                transform.rotation = Quaternion.identity;
                break;
        }
    }

    private void ApplyWallRotation(Vector2 throwDirection)
    {
        float zRotation = throwDirection.x < 0f ? -90f : 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    private void SnapToGround()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            Mathf.Infinity,
            LayerMask.GetMask(GameLayers.Ground)
        );

        if (rayHit.collider == null) return;

        transform.position = new Vector3(transform.position.x, rayHit.point.y, transform.position.z);
    }
}
