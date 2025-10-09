using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationAPI : AnimationAPI
{
    private readonly PlayerRenderer playerRenderer;
    public PlayerAnimationAPI(PlayerRenderer playerRenderer)
    {
        entityRenderer = playerRenderer;
        this.playerRenderer = playerRenderer;
    }

    public void Jump(int count) => playerRenderer.JumpAnim(count);
    public void Attack(int combo) => playerRenderer.AttackAnim(combo);
    public void Dash() => playerRenderer.DashAnim();
    public void Throw() => playerRenderer.ThrowAnim();
    //public void ClingWall(bool isClinging) => playerRenderer.ClingWallAnim(isClinging);
    public void Ladder(bool climb) => playerRenderer.LadderAnim(climb);

}
