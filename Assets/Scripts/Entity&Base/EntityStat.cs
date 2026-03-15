using System.Linq;
using UnityEngine;

[System.Serializable]
// 엔티티가 사용하는 현재 스탯과 추가 스탯 목록을 보관한다.
public class EntityStat
{
    [Header("Current Stats")]
    [SerializeField]
    private Stat hpStat;
    [SerializeField]
    private Stat staminaStat;

    [Header("Stats")]
    [SerializeField]
    private Stat[] stats;

    public Stat HPStat => hpStat;
    public Stat StaminaStat => staminaStat;
    public Stat GetStat(Stat stat) => stats.FirstOrDefault(x => x.StatType == stat.StatType);
    public Stat GetStat(StatType statType) => stats.FirstOrDefault(x => x.StatType == statType);
}