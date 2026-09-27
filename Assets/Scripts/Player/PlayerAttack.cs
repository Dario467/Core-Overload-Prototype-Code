using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;
    [SerializeField] private float attackCooldown;
    private bool isOnCooldown;  
    private bool isAttacking;

    
    [Header("Configuración de Audio")]
    [SerializeField] private AudioClip sonidoAtaque; 
    private AudioSource miAudioSource;

    void Awake()
    {
        miAudioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        hitbox.SetActive(false);
    }

    public void Attack()
    {
        isAttacking = true;
        Debug.Log("Attack!");

       
        if (sonidoAtaque != null && miAudioSource != null)
        {
            miAudioSource.PlayOneShot(sonidoAtaque);
        }
    }

    public void ActivateHitbox()
    {
        hitbox.SetActive(true);
    }

    public void DeactivateHitbox()
    {
        hitbox.SetActive(false);
    }

    public void EndAttack()
    {
        isAttacking = false;
        StartCoroutine(AttackCooldown());
    }

    public IEnumerator AttackCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown); 
        isOnCooldown = false;
    }

    public void InterruptAttack()
    {
        isAttacking = false;
        DeactivateHitbox();
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
}