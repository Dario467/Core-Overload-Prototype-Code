using System;
using System.Collections;
using UnityEngine;

public class AcaroAcorasado : MonoBehaviour, IAttack, IWander
{
    private EntityDash myDash;
    private Rigidbody2D rb;
    private Animator anim;

    private enemyAi ai;
    [SerializeField] private float chargeTime;
    
    private float AttackChargeTimer;
    private bool isAttacking;
    private bool isCharging;
    private bool reverse;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        myDash = GetComponent<EntityDash>();
        ai = GetComponent<enemyAi>();
    }
    void Start()
    {
        isAttacking = false;
        reverse = false;
    }

    void Update()
    {
        if (isCharging)
        {
            AttackTimer();
        }
        DashStopCondition();
    }

    public void DashStopCondition()
    {
        if (!myDash.IsDashing())
        {
            return;
        }

        if(reverse)
        {
            if(!ai.IsFloorBack())
            {
                myDash.StopDash();
            }
        }
        else
        {
            if (!ai.IsFloorAhead())
            {
                myDash.StopDash();
            }
        }
    }

    public void ExecuteWander()
    {
        anim.SetBool("IsMoving", true);
        rb.linearVelocity = new Vector2 (ai.GetMovingDirection().x*ai.GetSpeed(), rb.linearVelocity.y);
        if(!ai.IsFloorAhead() || ai.IsWallAhead())
        {
            anim.SetBool("IsMoving", false);
            ai.ChangeCurrentState(enemyAi.State.turn);
        }
    }

    public void ExecuteAttack()
    {
        if(!isAttacking)
        {
            isAttacking = true;
            Vector2 backVector = new Vector2(transform.localScale.x * -1, 0);
            anim.SetTrigger("Cover");
            myDash.TryTemporaryDash(backVector, 22f, 0.07f);
            reverse = true;
        }
    }

    public void ChargeAttack()
    {
        if (!isAttacking)
        {
            return;
        }
        AttackChargeTimer = Time.time + chargeTime + 0.05f;
        isCharging = true;
        AttackTimer();
    }

    private void AttackTimer()
    {
        if(Time.time >= AttackChargeTimer)
        {
            reverse = false;
            isCharging = false;
            Attack();
        }
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
        StartCoroutine(AttackSequence());
    }

    public IEnumerator AttackSequence()
    {
        myDash.TryDash(ai.GetMovingDirection());
        yield return new WaitForSeconds(myDash.GetDashDuration());
        reverse = true;
        myDash.TryDash(-ai.GetMovingDirection()*1.2f);
        yield return new WaitForSeconds(myDash.GetDashDuration());
        reverse = false;
        myDash.StopDash();
        anim.SetTrigger("ExitCover");
    }

    public void StopAttacking()
    {
        if(!isAttacking)
        {
            return;
        }
        ai.StartAttackCooldown();
        ai.ChangeToPrevState();
        isAttacking = false;
    }

    public void StopCharginAttack()
    {
        isAttacking = false;
    }

    
}
