using System.Collections;
using TarodevController;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HealingZone : MonoBehaviour
{
    [Header("Configuración General")]
    public float velocidadDeDrenado = 15f; 

    [Header("Configuración de Sonido")]
    [Tooltip("El sonido que sonará en bucle cuando el jugador se quede quieto aquí")]
    public AudioClip sonidoRecarga;
    public float delayBetweenSound= 0f;
    private bool isReproducing;
    private bool isIn;

    private Animator miAnimator;
    private AudioSource miAudioSource;

    private void Start()
    {
        miAnimator = GetComponent<Animator>();
        miAudioSource = GetComponent<AudioSource>();

        miAudioSource.clip = sonidoRecarga;
        miAudioSource.loop = false;
        miAudioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTimer playerTimer = other.GetComponent<PlayerTimer>();
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerTimer != null)
            {
                playerTimer.estaEnZonaDebilitante = true;
            }
            if(playerController != null)
            {
                playerController.RemoveDashCharge();
            }

            if (miAnimator != null)
            {
                miAnimator.SetBool("isRecovering", true);
            }
            GameManager.Instance.SetSpawnPoint(transform.position);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTimer playerTimer = other.GetComponent<PlayerTimer>();
            if (playerTimer != null)
            {
                playerTimer.RestarTiempo(velocidadDeDrenado * Time.deltaTime);
            }

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                bool estaQuieto = Mathf.Abs(rb.linearVelocity.x) < 0.1f && Mathf.Abs(rb.linearVelocity.y) < 0.1f;

                if (estaQuieto)
                {
                    if (!miAudioSource.isPlaying && !isReproducing) StartCoroutine(PlayWithDelay(miAudioSource,delayBetweenSound));
                    isIn = true;
                }
                else
                {
                    isIn= false;
                }
            }
        }
    }

    public IEnumerator PlayWithDelay(AudioSource source, float delay)
    {
        while (isIn)
        {
            isReproducing = true;
            source.Play();
            yield return new WaitForSeconds(0.1f);
            source.Play();
            yield return new WaitForSeconds(source.clip.length + delay);
            isReproducing = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTimer playerTimer = other.GetComponent<PlayerTimer>();
            if (playerTimer != null)
            {
                playerTimer.estaEnZonaDebilitante = false;
            }

            if (miAnimator != null)
            {
                miAnimator.SetBool("isRecovering", false);
            }

            if (miAudioSource != null && miAudioSource.isPlaying)
            {
                miAudioSource.Stop();
            }
        }
    }
}