using UnityEngine;

public interface IWeapon {
    void SetState(bool canAttack);
    void Shoot(Vector3 direction);
}
