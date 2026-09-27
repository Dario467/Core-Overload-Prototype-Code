using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    [SerializeField] private BoxCollider2D hurtboxCollider;

    public void HurtBoxActive()
    {
        if (hurtboxCollider != null)
        {
            hurtboxCollider.enabled = true;
        }
    }

    public void HurtBoxInactive()
    {
        if (hurtboxCollider != null)
        {
            hurtboxCollider.enabled = false;
        }
    }
}
