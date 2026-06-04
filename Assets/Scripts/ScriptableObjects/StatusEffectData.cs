using System;
using UnityEngine;

// 상태 이상 효과의 정적 설정값을 보관한다.
[CreateAssetMenu(menuName = "Effects/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    // 런타임에 생성된 EffectBase 인스턴스를 참조한다.
    [NonSerialized] public EffectBase effect;

    public EffectKind kind;
    public EffectTargetKind[] target;
    public float duration;
    public float rate;

    // 벽 방향 등 충돌 상황에 따라 런타임에서만 결정되는 값이다.
    [NonSerialized] public float xDir;
}

// 상태 이상 효과의 종류를 정의한다.
public enum EffectKind { Blind, Slow, Damage, WallJump, Stun, AttackDown }

// 상태 이상 효과가 적용될 대상을 정의한다.
public enum EffectTargetKind { Player, Enemy, Ground, Wall }
