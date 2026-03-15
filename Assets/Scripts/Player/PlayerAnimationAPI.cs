using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 애니메이션 제어 기능을 제공한다.
public class PlayerAnimationAPI : AnimationAPI
{
    private readonly PlayerRenderer playerRenderer;
    public PlayerAnimationAPI(PlayerRenderer playerRenderer)
    {
        entityRenderer = playerRenderer;
        this.playerRenderer = playerRenderer;
    }

    public void Jump(int count) => playerRenderer.JumpAnim(count);
    public void Dash() => playerRenderer.DashAnim();
    public void Throw() => playerRenderer.ThrowAnim();
    public void Ladder(bool isClimb) => playerRenderer.LadderAnim(isClimb);

    public void WallCling(bool isCling, float xDir = 0) => playerRenderer.WallClingAnim(isCling, xDir);

    public void PipeWarp(bool isStart, bool isLeftStart) => playerRenderer.PipeWarpAnim(isStart, isLeftStart);
}