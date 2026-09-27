using System;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace TarodevController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(AudioSource))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        public Animator anim;
        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        [SerializeField]private BoxCollider2D hurtBoxCollider;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;
        private bool dashWhileAttacking;
        private EntityDash dashScript;
        
        private PlayerStun stunScript; 
        private PlayerAttack attackScript;
        private PlayerTimer playerTimer;
        private bool hasDashCharge;
        
        private float horizontal; 
        public int facingDirection = 1; 
        [SerializeField] private LayerMask layer;

        [Header("Configuración de Audio")]
        [SerializeField] private AudioClip sonidoDash; 
        private AudioSource _audioSource;
        private bool _wasDashing; 

        [Header("Visuales de Dash")]
        [SerializeField] private GameObject imagenDashListo; 
        [SerializeField] private GameObject imagenDashVacio;
        
        #region Interface
        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        #endregion

        private float _time;
        public bool accertAttack = false;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

            _audioSource = GetComponent<AudioSource>();

            dashScript = GetComponent<EntityDash>();
            
            stunScript = GetComponent<PlayerStun>();
            attackScript = GetComponent<PlayerAttack>();
            playerTimer = GetComponent<PlayerTimer>();
        }

        void Start()
        {
            GameManager.Instance.PlayerTransform = transform;
            transform.position = GameManager.Instance.GetSpawnPoint();
            dashWhileAttacking = false;
            hasDashCharge = false;

            ActualizarIndicadorDash();
        }

        private void Update()
        {
            if (SceneLoader.IsLoading) return;
            _time += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.Instance.RestartLevel();
            }
            GatherInput();
            if(dashWhileAttacking && !attackScript.IsAttacking() && dashScript != null)
            {
                if (hurtBoxCollider != null)
                {
                    hurtBoxCollider.enabled = false;
                }
                _frameVelocity = Vector2.zero;
                Vector2 direccionDash = _frameInput.Move != Vector2.zero ? _frameInput.Move.normalized : new Vector2(facingDirection, 0);
                transform.localScale = new Vector3(direccionDash.x, transform.localScale.y, transform.localScale.z);
                if(direccionDash.y != 0)
                {
                    direccionDash.x = 0;
                }
                dashScript.TryDash(direccionDash);
                hasDashCharge = false;

                ActualizarIndicadorDash();

                dashWhileAttacking = false;
            }
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
        }

        private void FixedUpdate()
        {   
            bool isDashingRightNow = dashScript != null && dashScript.IsDashing();

            if (isDashingRightNow && !_wasDashing)
            {
                if (sonidoDash != null) _audioSource.PlayOneShot(sonidoDash);
            }
            _wasDashing = isDashingRightNow; 

            if (isDashingRightNow && !attackScript.IsAttacking())
            {
                anim.SetBool("isDashing", true);
                return;
            }

            HandleDirection();
            CheckCollisions();
            
            HandleJump();
            ApplyMovement();

            horizontal = _frameVelocity.x; 
            
            if (anim != null) 
            {
                anim.SetFloat("horizontal", Mathf.Abs(horizontal)); 
                anim.SetBool("isGrounded", _grounded); 
                anim.SetBool("isDashing", false);
            }
            if (hurtBoxCollider != null && !hurtBoxCollider.enabled)
            {
                hurtBoxCollider.enabled = true;
            }

            if (attackScript.IsAttacking())
            {
                _frameInput.Move.x = 0f;
            }

            if(isDashingRightNow)
            {
                _rb.gravityScale = 0f;
            }else{
                _rb.gravityScale = 1f;
                HandleGravity();
            }

            if (_frameInput.Move.x > 0) facingDirection = 1;
            else if (_frameInput.Move.x < 0) facingDirection = -1;

            Vector3 characterScale = transform.localScale;
            characterScale.x = Mathf.Abs(characterScale.x) * facingDirection;
            transform.localScale = characterScale;

            bool tocandoPared = Physics2D.Raycast(transform.position, new Vector2(facingDirection, 0), 0.6f, layer);

            if (tocandoPared && !_grounded) _frameVelocity.x = 0; 
        }

        #region Collisions
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, layer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, layer);
            
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }
        #endregion

        #region Jumping
        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;
            if (!_jumpToConsume && !HasBufferedJump) return;
            if (_grounded || CanUseCoyote) ExecuteJump();
            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }
        #endregion

        #region Horizontal
        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }
        #endregion

        #region Gravity
        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
                if(accertAttack){
                    accertAttack = false;
                }
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if(dashScript != null && attackScript.IsAttacking() && accertAttack)
                {
                    inAirGravity = 1f;
                    _frameVelocity.y = 0;
                    Debug.Log($"y=0");
                }
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }
        #endregion

        private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;

        public void OnDash(InputValue value)
        {
            if(attackScript.IsAttacking() && hasDashCharge)
            {
                dashWhileAttacking = true;
                return;
            }
            if (value.isPressed && dashScript != null && hasDashCharge)
            {
                if (hurtBoxCollider != null)
                {
                    hurtBoxCollider.enabled = false;
                }
                playerTimer.RestarTiempo(1f);
                hasDashCharge = false;

                ActualizarIndicadorDash();

                _frameVelocity = Vector2.zero;
                Vector2 direccionDash = _frameInput.Move != Vector2.zero ? _frameInput.Move.normalized : new Vector2(facingDirection, 0);
                if(direccionDash.y != 0)
                {
                    direccionDash.x = 0;
                }
                if (dashScript != null) dashScript.TryDash(direccionDash);
            }
        }

        public void OnAttack(InputValue value)
        {
            if (value.isPressed && attackScript != null && !attackScript.IsAttacking() && !attackScript.IsOnCooldown())
            {
                if (Mathf.Abs(_frameVelocity.x) > 0f)
                {
                    _frameVelocity.x = 0f;
                }

                if (dashScript != null && dashScript.IsDashing())
                {
                    Debug.Log("Attack while dashing, stopping dash.");
                    dashScript.StopDash(); 
                }
                anim.SetBool("isDashing", false);
                attackScript.Attack();
                anim.SetTrigger("isAttacking");
          }
        }

        public void AddDashCharge()
        {
            hasDashCharge = true;

            ActualizarIndicadorDash();
        }

        public void RemoveDashCharge()
        {
            hasDashCharge = false;
            ActualizarIndicadorDash();
        }

        private void ActualizarIndicadorDash()
        {
            if (imagenDashListo != null) imagenDashListo.SetActive(hasDashCharge);
            if (imagenDashVacio != null) imagenDashVacio.SetActive(!hasDashCharge);
        }

        public void ResetPlayer()
        {
            if (dashScript.IsDashing())
            {
                dashScript.StopDash();
            }
            if (attackScript.IsAttacking())
            {
                attackScript.InterruptAttack();
            }
            anim.SetBool("isDashing", false);
        }

        void OnDisable()
        {
            dashScript.StopDash();
            anim.SetBool("isDashing", false);
            attackScript.InterruptAttack();
        }

        public void SetAccertAttackTrue(){
            this.accertAttack = true;
        }

        public bool IsGrounded() => _grounded;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}