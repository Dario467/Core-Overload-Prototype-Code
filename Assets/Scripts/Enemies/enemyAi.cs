using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class enemyAi : MonoBehaviour
{
    private Rigidbody2D rb;
    private IAttack attack;
    private IWander wander;
    [SerializeField]private LayerMask playerLayer;

    private Vector2 movingDirection;
    private float turnTimer;

    [SerializeField] private LayerMask plataformLayer;
    [SerializeField] private float speed;
    [SerializeField] private float turnCoolDown = 0.2f;

    [SerializeField]private float chaseDetectionDistance;
    [SerializeField] private float attackDetectionDistance;

    [SerializeField] private float timeBetweenAttacks;
    private float attackTimer;
    private bool aiActive;

    public enum State
    {
        wander,
        turn,
        chase,
        attacking,
        evade
    }
    private bool isTurning = false;
    private bool endStun;
    private bool CanStartAttackTimer;
    private State prevState;
    private State currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        attack = GetComponent<IAttack>();
        wander = GetComponent<IWander>();
        movingDirection = new Vector2(transform.localScale.x, 0);
    }

    void Start()
    {
        aiActive = true;
        endStun = true;
        CanStartAttackTimer = true;
        currentState = State.wander;
        prevState = currentState;
    }

    void Update()
    {
        if (GameManager.Instance.IsPlayerStunned)
        {
            rb.linearVelocity = Vector2.zero;
            aiActive = false;
            endStun = false;
        }
        if (!GameManager.Instance.IsPlayerStunned && !aiActive && !endStun)
        {
            aiActive = true;
            endStun = true;
        }
        if(movingDirection.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movingDirection.x),transform.localScale.y,transform.localScale.z);
        }
        if(!aiActive)
        {
            Debug.Log("AI Inactive");
            return;
        }
        else
        {
            Vector2 originPos = new Vector2(transform.position.x+(0.3f*transform.localScale.x),transform.position.y-0.2f);
            Debug.DrawRay(originPos, Vector2.down*0.8f, Color.red);
            ChaseListener();
            WanderListener();
            AttackListener();
            
            StateMachine();
        }
    }

    public void StateMachine()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case State.wander:
                wander.ExecuteWander();
                break;
            case State.chase:
                Chase();
                break;
            case State.turn:
                Turn();
                break;
            case State.attacking:
                attack.ExecuteAttack();
                break;
            default:
                wander.ExecuteWander();
                break;
        } 
    }

    public void Chase()
    {
        if(speed == 0)
        {
            return;
        }
        float directionToPlayerX = Math.Sign(GameManager.Instance.PlayerTransform.position.x - transform.position.x);
        if(directionToPlayerX != movingDirection.x)
        {
            prevState = currentState;
            ChangeCurrentState(State.turn);
            return;
        }
        if (IsFloorAhead())
        {
            rb.linearVelocity = new Vector2 (movingDirection.x*speed, rb.linearVelocity.y);
        }
         else
        {
            rb.linearVelocity = new Vector2(0,rb.linearVelocity.y);
        }
    }

    public void Turn()
    {
        if (!isTurning)
        {
            rb.linearVelocity = new Vector2(0,rb.linearVelocity.y);
            isTurning = true;
            turnTimer = Time.time+turnCoolDown;
        }
        if(Time.time >= turnTimer)
        {
            movingDirection = new Vector2(-movingDirection.x,0);
            isTurning = false;
            currentState = prevState;
        }
    }

    public bool IsWallAhead()
    {
        Vector2 wallRayDir = new Vector2(transform.localScale.x, 0);
        Debug.DrawRay(transform.position, wallRayDir*0.6f, Color.yellow);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, wallRayDir, 0.6f, plataformLayer);
        return hit.collider != null;;
    }

    public bool IsFloorAhead()
    {
        Vector2 originPos = new Vector2(transform.position.x+(0.3f*transform.localScale.x),transform.position.y-0.2f);
        Debug.DrawRay(originPos, Vector2.down*0.8f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(originPos, Vector2.down, 0.8f, plataformLayer);
        return hit.collider != null;
    }

    public bool IsFloorBack()
    {
        Vector2 originPos = new Vector2(transform.position.x-(0.3f*transform.localScale.x),transform.position.y-0.2f);
        Debug.DrawRay(originPos, Vector2.down*0.5f, Color.blue);
        RaycastHit2D hit = Physics2D.Raycast(originPos, Vector2.down, 0.5f, plataformLayer);
        return hit.collider != null;
    }

    public Vector2 GetMovingDirection()
    {
        return movingDirection;
    }

    public void SetLinearVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public float GetSpeed()
    {
        return speed;
    }

    private void ChaseListener()
    {
        if(chaseDetectionDistance == 0)
        {
            return;
        }
        if(currentState == State.chase || currentState == State.turn)
        {
            return;
        }

        float distanceToPlayer = (GameManager.Instance.PlayerTransform.position - transform.position).magnitude;
       
        Vector2 directionToPlayer = (GameManager.Instance.PlayerTransform.position - transform.position).normalized;
        Debug.DrawRay(transform.position, directionToPlayer*chaseDetectionDistance, Color.black);
        if(distanceToPlayer <= chaseDetectionDistance && GameManager.Instance.PlayerTransform.position.y <= transform.position.y+4.5f)
        {
            ChangeCurrentState(State.chase);
            Debug.Log("Player Detected");
        }
    }
    
public void AttackListener()
    {
        if(attack == null || attackDetectionDistance == 0)
        {
            return;
        }
        if(currentState == State.attacking)
        {
            return;
        }
        if(Time.time < attackTimer)
        {
            return;
        }
       
        Debug.DrawRay(transform.position, movingDirection*attackDetectionDistance, Color.yellow);
        RaycastHit2D enemyRay = Physics2D.Raycast(transform.position, movingDirection, attackDetectionDistance, playerLayer);
        Debug.Log("Attack listener");
        if (enemyRay.collider == null)
        {
            Debug.Log("No hit");
        }
        if(enemyRay.collider != null)
        {
            if (CanStartAttackTimer)
            {
                attackTimer = Time.time + timeBetweenAttacks;
                CanStartAttackTimer = false;
            }
            else
            {
                ChangeCurrentState(State.attacking);
            }
        }
    }

    private void WanderListener()
    {
        if(currentState == State.wander || currentState == State.turn)
        {
            return;
        }

        float distanceToPlayer = (GameManager.Instance.PlayerTransform.position - transform.position).magnitude;
        if(distanceToPlayer >= chaseDetectionDistance/0.5f || GameManager.Instance.PlayerTransform.position.y > transform.position.y+1.5f)
        {
            Debug.Log("Player Lost");
            ChangeCurrentState(State.wander);
        }
    }

    public void ChangeCurrentState(State newState)
    {
        if (currentState == newState)
        {
            return;
        }
        if(currentState == State.turn)
        {
            return;
        }
        if(currentState == State.attacking && newState != State.turn)
        {
            return;
        }

        prevState = currentState;
        currentState = newState;
    }

    public void ChangeToPrevState()
    {
        currentState = prevState;
    }

    public void StartAttackCooldown()
    {
        CanStartAttackTimer = true;
    }

    public void AI(bool active)
    {
        aiActive = active;
    }


}
