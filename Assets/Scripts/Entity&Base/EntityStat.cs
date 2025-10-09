using System.Linq;
using UnityEngine;

[System.Serializable]
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
    public Stat GetStat(Stat stat) => stats.FirstOrDefault(x => x.StatType == stat.StatType); // 첫 번째 요소 또는 조건에 부합하는 첫 번째 요소
    public Stat GetStat(StatType statType) => stats.FirstOrDefault(x => x.StatType == statType);
}
