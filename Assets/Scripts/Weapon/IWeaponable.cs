using UnityEngine;

// IWeaponable 인터페이스를 정의한다.
public interface IWeaponable
{
    void GetWeapon();
    void PutWeapon();
    void OnAttack();
    void OnThrow(Vector2 throwPosition);
}