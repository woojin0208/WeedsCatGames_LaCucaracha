using UnityEngine;

// 상태 이상 효과의 설정값을 저장하는 에셋이다.
[CreateAssetMenu(menuName = "Effects/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public EffectBase effect;
    public EffectKind kind;
    public EffectTargetKind[] target;
    public float duration;
    public float rate;

    public float xDir;
}

// 상태 이상 효과의 종류를 정의한다.
public enum EffectKind { Blind, Slow, Damage, WallJump, }

// 상태 이상 효과가 적용될 대상을 정의한다.
public enum EffectTargetKind { Player, Enemy, Ground, Wall}