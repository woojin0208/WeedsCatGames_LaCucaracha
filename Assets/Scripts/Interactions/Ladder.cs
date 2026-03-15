using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사다리 상호작용을 통해 플레이어 이동을 시작한다.
public class Ladder : MonoBehaviour, IInteractable
{
    [Header("Ladder Setting")]
    [SerializeField]
    private Transform startPosition;
    [SerializeField]
    private Transform endPosition;

    [SerializeField] private AudioSource audioSource;

    [field: SerializeField] public Transform InteractivePos { get; set; }

    public void Interactive(PlayerBase player)
    {
        if (audioSource != null) audioSource.Play(); 

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController.transform.position.y > this.transform.position.y)
            playerController.OnLadder(endPosition.position, startPosition.position);
        else 
            playerController.OnLadder(startPosition.position, endPosition.position);

        Invoke(nameof(EndLadder), 1.5f);
    }

    private void EndLadder() => audioSource?.Stop();
}