using Cinemachine;
using UnityEngine;

// 플레이어의 상대 높이에 따라 카메라 Y 추적을 켜고 끈다.
public class CinemachineYThreshold : MonoBehaviour
{
    [Header("플레이어 및 임계치")]
    [SerializeField]
    private Transform player;
    private PlayerMovement playerMovement;

    [Tooltip("카메라가 따라오기 시작하는 높이 기준값이다.")]
    public float jumpOffsetUp = 1.5f;
    [Tooltip("내려올 때 이 높이 아래로 오면 카메라 추적을 멈춘다.")]
    public float jumpOffsetDown = 1.0f;

    [Header("비활성화 시 데드/소프트 존")]
    [Tooltip("Y 추적이 꺼졌을 때 사용할 데드존 높이다.")]
    [SerializeField] private float freezeDeadZoneY = 1f;
    [Tooltip("Y 추적이 꺼졌을 때 사용할 소프트존 높이다.")]
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

        // 상승 중에는 상단 임계치, 하강 중에는 하단 임계치로 추적 여부를 판정한다.
        bool wasFollowing = isFollowing;
        isFollowing = wasFollowing ? (relativeY >= jumpOffsetDown) : (relativeY > jumpOffsetUp);

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
        // 존이 화면 전체를 차지하므로 Y 댐핑은 0으로 둔다.
        transposer.m_YDamping = 0;
    }

    private void RestoreCameraY()
    {
        transposer.m_TrackedObjectOffset = originTrackedOffset;
        transposer.m_DeadZoneHeight = originDeadZoneY;
        transposer.m_SoftZoneHeight = originSoftZoneY;
        transposer.m_YDamping = originYDamping;
    }
}