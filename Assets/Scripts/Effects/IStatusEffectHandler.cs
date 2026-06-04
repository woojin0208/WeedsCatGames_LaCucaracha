// 상태 이상 효과를 적용하고 해제할 수 있는 대상 규약을 정의한다.
public interface IStatusEffectHandler
{
    void ApplyEffect(StatusEffectData effectData);

    void IgnoreEffect(StatusEffectData effectData);
}
