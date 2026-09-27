using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour, IBullet
{
    [Header("Configuración de Bala")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; 
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Shoot(Vector2 direction, Vector2 position)
    {
        transform.position = position;
        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Confiner") )
        {
            return; 
        }

        Debug.Log("La bala choco con " + collision.gameObject.name);
        Destroy(gameObject);
    }
}