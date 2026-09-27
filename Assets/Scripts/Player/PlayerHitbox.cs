using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions.Comparers;
using TarodevController;

public class PlayerHitbox : MonoBehaviour
{
    [SerializeField] private float playerDamage = 0f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private LayerMask enemyLayer;
    private HashSet<IDamageable> hitDamageable = new HashSet<IDamageable>();

    [SerializeField]private float timeReducedByHit = 5f; // time in seconds to reduce the PlayerTimer.

    private PlayerTimer playerTimer;
    private PlayerController playerController;


    void Awake()
    {
        playerTimer = GetComponentInParent<PlayerTimer>();
        playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyLayer) == 0)
        {
            return;
        }
        var damageable = collision.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            if (!hitDamageable.Contains(damageable))
            {
                playerTimer.RestarTiempo(timeReducedByHit);
                if (collision.CompareTag("TwoPointer"))
                {
                    Debug.Log("two points hit");
                    playerTimer.RestarTiempo(timeReducedByHit);
                }
                playerController.AddDashCharge();
                damageable.TakeDamage(playerDamage);
                damageable.AplyKnockback(new Vector2(GameManager.Instance.PlayerTransform.localScale.x, 0), knockbackForce);
                playerController.SetAccertAttackTrue();
                hitDamageable.Add(damageable);
            }
        }
    }

    private void OnDisable()
    {
        hitDamageable.Clear();
    }
}
