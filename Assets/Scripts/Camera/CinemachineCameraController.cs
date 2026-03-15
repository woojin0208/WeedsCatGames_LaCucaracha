using UnityEngine;
using Cinemachine;

// 점프 높이에 따라 시네머신 카메라의 Y 추적 시작 시점을 제어한다.
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineCameraController : MonoBehaviour
{
    [Header("플레이어 레퍼런스")]
    [SerializeField] private Transform player;
    private PlayerMovement playerMovement;

    [Header("점프 오프셋")]
    [Tooltip("땅에서 이만큼 위로 올라가야 카메라가 Y 추적을 시작한다.")]
    [SerializeField] private float jumpOffsetUp = 1.5f;

    private CinemachineFramingTransposer _transposer;
    private float _origOffsetY;
    private float _groundY;
    private bool _hasExceeded;

    void Awake()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        var vcam = GetComponent<CinemachineVirtualCamera>();
        _transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        _origOffsetY = _transposer.m_TrackedObjectOffset.y;

        // 시작 시점의 지면 높이를 기준값으로 기록한다.
        _groundY = player.position.y;
        _hasExceeded = false;
    }

    void LateUpdate()
    {
        // 착지하면 지면 기준 높이를 다시 잡고 추적 상태를 초기화한다.
        if (playerMovement != null && playerMovement.IsGrounded)
        {
            _groundY = player.position.y;
            _hasExceeded = false;
            return;
        }

        float relativeY = player.position.y - _groundY;

        // 처음 임계 높이를 넘었을 때만 추적 시작 플래그를 세운다.
        if (!_hasExceeded && relativeY > jumpOffsetUp)
        {
            _hasExceeded = true;
        }

        // 임계 높이 아래로 내려오면 원래 오프셋으로 복귀한다.
        if (_hasExceeded && relativeY <= jumpOffsetUp)
        {
            var offset = _transposer.m_TrackedObjectOffset;
            offset.y = _origOffsetY;
            _transposer.m_TrackedObjectOffset = offset;
            _hasExceeded = false;
        }
    }
}