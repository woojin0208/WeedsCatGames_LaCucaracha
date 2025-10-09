using Cinemachine;
using UnityEngine;

public class CinemachineYThreshold : MonoBehaviour
{
    [Header("플레이어 및 임계치")]
    [SerializeField]
    private Transform player;
    private PlayerMovement playerMovement;

    [Tooltip("카메라가 따라올 정도의 높이")]
    public float jumpOffsetUp = 1.5f;
    [Tooltip("내려올 때 이만큼 아래로 내려오면 카메라가 멈춤")]
    public float jumpOffsetDown = 1.0f;

    [Header("비활성화 시 데드/소프트 존")]
    [Tooltip("Y 추적 꺼졌을 때의 dead 값")]
    [SerializeField] private float freezeDeadZoneY = 1f;
    [Tooltip("Y 추적 꺼졌을 때의 soft 값")]
    [SerializeField] private float freezeSoftZoneY = 0f;

    [SerializeField]
    private float rayDistance = 1.0f;

    private CinemachineFramingTransposer transposer;
    private float originDeadZoneY, originSoftZoneY, originYDamping;
    private float groundY;
    private bool isFollowing;
    Vector3 originTrackedOffset;
    private void Awake()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        var vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        originTrackedOffset = transposer.m_TrackedObjectOffset;
        originDeadZoneY = transposer.m_DeadZoneHeight;
        originSoftZoneY = transposer.m_SoftZoneHeight;
        originYDamping = transposer.m_YDamping;

        groundY = player.position.y;
        isFollowing = false;

    }

    private float GetGroundY()
    {
        Vector2 origin = player.position + Vector3.up * 0.1f;
        RaycastHit2D rayHit = Physics2D.Raycast(origin, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        return (rayHit.collider != null) ? rayHit.point.y : groundY;
    }

    private void LateUpdate()
    {
        groundY = GetGroundY();

        if (playerMovement.IsGrounded)
        {
            isFollowing = false;
            groundY = player.position.y;
            FreezeCameraY();
            return;
        }


        float relativeY = (player.position.y + originTrackedOffset.y) - groundY;

        bool wasFollowing = isFollowing;
        isFollowing = wasFollowing ? (relativeY >= jumpOffsetDown) : (relativeY > jumpOffsetUp);    // 올라갈 때 기준

        if (isFollowing) RestoreCameraY();
        else FreezeCameraY();
    }

    private void FreezeCameraY()
    {
        transposer.m_TrackedObjectOffset = new Vector3(
             originTrackedOffset.x,
             groundY - player.position.y + originTrackedOffset.y,
             originTrackedOffset.z
        );

        transposer.m_DeadZoneHeight = 1f;
        transposer.m_SoftZoneHeight = 1f;
        transposer.m_YDamping = 0;  // zone이 전체이므로 damping은 크게 상관없음
    }

    private void RestoreCameraY()
    {
        transposer.m_TrackedObjectOffset = originTrackedOffset;
        transposer.m_DeadZoneHeight = originDeadZoneY;
        transposer.m_SoftZoneHeight = originSoftZoneY;
        transposer.m_YDamping = originYDamping;
    }
}
