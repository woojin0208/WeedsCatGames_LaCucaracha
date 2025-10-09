using UnityEngine;

public interface IWeaponable
{
    
    void GetWeapon();
    void PutWeapon();
    void OnAttack();
    void OnThrow(Vector2 throwPosition);
}
