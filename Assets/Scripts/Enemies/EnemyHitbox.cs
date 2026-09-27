using UnityEngine;
using UnityEngine.UIElements;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField]private LayerMask targetLayer; 
    [SerializeField]private float StunTime = 10;
    private BoxCollider2D hitbox;

    void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            var stuneable = collision.GetComponentInParent<IStuneable>();
            if (stuneable != null)
            {
                stuneable.ActivarStun(StunTime);
            }
        }
    }

    public void DesactivateHitbox()
    {
        hitbox.enabled = false;
    }
    public void ActivateHitbox()
    {
        hitbox.enabled = true;
    }
}
