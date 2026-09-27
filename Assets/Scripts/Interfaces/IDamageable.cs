using UnityEngine;
public interface IDamageable
{
    void TakeDamage(float damage);
    void AplyKnockback(Vector2 knockbackDirection, float knockbackForce);
}