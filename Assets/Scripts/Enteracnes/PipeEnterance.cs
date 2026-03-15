using System.Collections;
using UnityEngine;

// 파이프 이동 연출 후 씬 전환을 시작하는 입장 지점이다.
public class PipeEnterance : InteractableEnterance
{
    [field: SerializeField] public bool IsLeftStart { get; private set; }
    [field : SerializeField] public float XOffset { get; private set; } = 0.25f;
    public override void Interactive(PlayerBase player)
    {
        if (player.GetComponent<PlayerController>().TryPipeWarp(true, this))
            StartCoroutine(PipeWarpCoroutine(player));
    }

    private IEnumerator PipeWarpCoroutine(PlayerBase player)
    {
        yield return new WaitForSeconds(0.5f);

        base.EnterArea(nextArea, EnteranceType.Pipe);
    }

}