using UnityEngine;

public class PlataformHitbox : MonoBehaviour
{
    [SerializeField]private LayerMask targetLayer; 
    [SerializeField]private float StunTime = 10;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            var stuneable = collision.GetComponentInParent<IStuneable>();
            if (stuneable != null)
            {
                stuneable.UpWhileStun();
                stuneable.ActivarStun(StunTime);
            }
        }
    }
}
