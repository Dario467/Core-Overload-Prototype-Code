using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EntityStats: MonoBehaviour, IDamageable
{
     [SerializeField] private float maxHealth;
     [SerializeField] private float KnockbackStunDuration=0.1f;
     private float currentHealth;

     private Rigidbody2D rb;
     private EntityFlash entityFlash;
     private enemyAi ai;
     private SpriteRenderer spriteRenderer;
     [SerializeField] private float knockBackMultiplier= 1f;

     void Awake()
     {
        rb = GetComponent<Rigidbody2D>();
        entityFlash = GetComponent<EntityFlash>();
        ai = GetComponent<enemyAi>();
        spriteRenderer = GetComponent<SpriteRenderer>();
     }

    void Start()
    {
        spriteRenderer.flipY = false;
        spriteRenderer.color = Color.white;
        currentHealth = maxHealth;   
    }

    public void AplyKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        Debug.Log("Applying Knockback");
        if(knockBackMultiplier == 0)
        {
            return;
        }
        StartCoroutine(KnockbackCoroutine(knockbackDirection, knockbackForce*knockBackMultiplier));
    }

    private IEnumerator KnockbackCoroutine(Vector2 knockbackDirection, float knockbackForce)
    {
         if (rb != null)
        {
            ai.AI(false);
            //yield return new WaitForSeconds(0.1f);
            Debug.Log("Knockback Applied");
            rb.linearVelocity = Vector2.zero;
            rb.linearVelocity = knockbackDirection.normalized * knockbackForce;
            yield return new WaitForSeconds(0.1f);
            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(KnockbackStunDuration);
            ai.AI(true);
        }
    }

    public void TakeDamage(float damage)
    {
        if (entityFlash != null)
        {
            entityFlash.ExecuteImpact();
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if(spriteRenderer != null)
            {
                spriteRenderer.flipY = true;
                spriteRenderer.color = Color.red;
            }
            StartCoroutine(DeathCoroutine());
        }
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    void OnEnable()
    {
        spriteRenderer.flipY = false;
        spriteRenderer.color = Color.white;
    }
}
