using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public EffectKind kind;
    public EffectTargetKind[] target;
    public float duration;
    public float rate;
}

public enum EffectKind { Blind, Slow, Damage, WallJump, }

public enum EffectTargetKind { Player, Enemy, Ground, Wall}