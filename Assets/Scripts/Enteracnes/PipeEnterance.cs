using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeEnterance : InteractableEnterance
{
    public override void Interactive(PlayerBase player)
    {
        player.GetComponent<PlayerController>().TryPipeWarp(true, transform.position);

        PipeWarpCoroutine(player);
    }

    private IEnumerator PipeWarpCoroutine(PlayerBase player)
    {
        yield return new WaitForSeconds(0.5f);

        base.EnterArea(nextArea);
    }
}
