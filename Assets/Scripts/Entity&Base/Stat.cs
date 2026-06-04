using UnityEngine;

[System.Serializable]
// 단일 스탯 값과 변경 이벤트를 보관한다.
public class Stat
{
    public delegate void ValueChangedHandler(Stat stat, float prev, float current);
    public event ValueChangedHandler OnValueChanged;
    public event ValueChangedHandler OnValueMax;
    public event ValueChangedHandler OnValueMin;

    [SerializeField]
    private StatType statType;
    [SerializeField]
    private float maxValue;
    [SerializeField]
    private float minValue;
    [SerializeField]
    private float defaultValue;
    [SerializeField]
    private float bonusValue;

    public void CopyData(Stat newStat)
    {
        statType = newStat.statType;
        maxValue = newStat.maxValue;
        minValue = newStat.minValue;
        defaultValue = newStat.defaultValue;
        bonusValue = newStat.bonusValue;
    }

    public StatType StatType => statType;
    public float MaxValue => maxValue;
    public float MinValue => minValue;
    public float Value => Mathf.Clamp(defaultValue + bonusValue, minValue, maxValue);

    public float DefaultValue
    {
        get => defaultValue;
        set
        {
            float prev = Value;
            defaultValue = Mathf.Clamp(value, minValue, maxValue);
            TryInvokeValueChangedEvent(prev, Value);
        }
    }

    public float BonusValue
    {
        get => bonusValue;
        set
        {
            float prev = Value;
            bonusValue = Mathf.Clamp(value, minValue - defaultValue, maxValue - defaultValue);
            TryInvokeValueChangedEvent(prev, Value);
        }
    }

    private void TryInvokeValueChangedEvent(float prev, float current)
    {
        if (Mathf.Approximately(prev, current)) return;

        OnValueChanged?.Invoke(this, prev, current);
        if (Mathf.Approximately(current, maxValue)) OnValueMax?.Invoke(this, prev, maxValue);
        else if (Mathf.Approximately(current, minValue)) OnValueMin?.Invoke(this, prev, minValue);
    }
}

// 프로젝트에서 사용하는 스탯 종류를 정의한다.
public enum StatType { AttackDamage = 0, HP, Stamina, MoveSpeed, DashSpeed, JumpForce, AttackSpeed, RecoverHP, RecoverStamina, }