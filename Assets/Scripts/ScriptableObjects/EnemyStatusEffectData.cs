using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Enemy Status Effect")]
public class EnemyStatusEffectData : ScriptableObject
{
    public EffectKind kind;
    public float duration;
    public float rate;
}

public enum EnemyEffectKind { Blind, Slow, Damage }
