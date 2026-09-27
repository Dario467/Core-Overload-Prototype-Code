using UnityEngine;
using TarodevController;
using UnityEngine.InputSystem;

public class PlayerStun : MonoBehaviour, IStuneable
{
    private float tiempoStuneadoMaximo = 3f;

    [Header("Caída en Stun")]
    [SerializeField]private float velocidadCaidaStun = 5f;

    public bool EstaStuneado { get; private set; }

    private float tiempoTranscurrido;
    private PlayerController playerController;
    private PlayerInput playerInput;
    private Animator anim;
    private Rigidbody2D rb;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GameManager.Instance.IsPlayerStunned = EstaStuneado;
        if (!EstaStuneado) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.RestartLevel();
        }

        tiempoTranscurrido += Time.deltaTime;

        if (rb != null && tiempoTranscurrido <= 0.2f)
        {      
            rb.linearVelocity = new Vector2(transform.localScale.x*-15f, -velocidadCaidaStun);
        }
        else
        {
             rb.linearVelocity = new Vector2(0, -velocidadCaidaStun);
        }

        if (tiempoTranscurrido >= tiempoStuneadoMaximo)
            TerminarStun();
    }

    public void ActivarStun(float duracion)
    {
        if (EstaStuneado) return;

        EstaStuneado = true;
        anim.SetBool("isStun", true);
        tiempoStuneadoMaximo = duracion;
        tiempoTranscurrido = 0f;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        SetComponentesActivos(false);
    }

    public void UpWhileStun()
    {
        
    }

    private void TerminarStun()
    {
        anim.SetBool("isStun", false);
        EstaStuneado = false;
        SetComponentesActivos(true);
    }

    private void SetComponentesActivos(bool activo)
    {
        if (playerController != null) playerController.enabled = activo;
        if (playerInput != null) playerInput.enabled = activo;
    }
}