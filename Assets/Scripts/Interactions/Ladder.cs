using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [Header("Ladder Setting")]
    [SerializeField]
    private Transform startPosition;
    [SerializeField]
    private Transform endPosition;

    [field: SerializeField] public Transform InteractivePos { get; set; }

    public void Interactive(EntityBase entity)
    {
        PlayerController playerController = entity.GetComponent<PlayerController>();
        if (playerController.transform.position.y > this.transform.position.y)
            playerController.OnLadder(endPosition.position, startPosition.position);
        else 
            playerController.OnLadder(startPosition.position, endPosition.position);
    }
}
