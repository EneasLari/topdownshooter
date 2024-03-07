using UnityEngine;


public interface IDamageable
{

    void TakeHit(float damage, Vector3 hitpoint, Vector3 hitDirection);
    void TakeDamage(float damage);
}