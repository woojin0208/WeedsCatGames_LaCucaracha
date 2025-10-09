using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineCameraController : MonoBehaviour
{
    [Header("플레이어 레퍼런스")]
    [SerializeField] private Transform player;
    private PlayerMovement playerMovement;  // 지면 감지용

    [Header("점프 오프셋")]
    [Tooltip("땅에서 이만큼↑ 올라가야 카메라가 Y추적 시작")]
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

        // 초기 지면 높이 설정
        _groundY = player.position.y;
        _hasExceeded = false;
    }

    void LateUpdate()
    {
        // 착지 시 groundY 갱신 및 플래그 리셋
        if (playerMovement != null && playerMovement.IsGrounded)
        {
            _groundY = player.position.y;
            _hasExceeded = false;
            return;
        }

        // 상대 높이 계산
        float relativeY = player.position.y - _groundY;

        // 문턱을 처음 넘어설 때 플래그 세팅
        if (!_hasExceeded && relativeY > jumpOffsetUp)
        {
            _hasExceeded = true;
        }

        // 문턱을 넘었다가 다시 내려왔을 때 원위치 스냅
        if (_hasExceeded && relativeY <= jumpOffsetUp)
        {
            var offset = _transposer.m_TrackedObjectOffset;
            offset.y = _origOffsetY;
            _transposer.m_TrackedObjectOffset = offset;
            _hasExceeded = false;
        }
    }
}
