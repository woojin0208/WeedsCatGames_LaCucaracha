using System.Collections;
using UnityEngine;

// 파이프 이동 연출 후 씬 전환을 시작하는 입장 지점이다.
public class PipeEnterance : InteractableEnterance
{
    [field: SerializeField] public bool IsLeftStart { get; private set; }
    [field: SerializeField] public float XOffset { get; private set; } = 0.25f;

    public override void Interactive(PlayerBase player)
    {
        if (player == null)
        {
            Debug.LogWarning("[PipeEnterance] player 가 null 입니다.", this);
            return;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("[PipeEnterance] PlayerController 가 null 입니다.", this);
            return;
        }

        if (playerController.TryPipeWarp(true, this))
        {
            StartCoroutine(PipeWarpCoroutine());
        }
    }

    private IEnumerator PipeWarpCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        base.EnterArea(EnteranceType.Pipe);
    }

}