using System.Collections;
using UnityEngine;

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
