using UnityEngine;

public class EntityDash : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private float dashTimer;
    private bool isDashing;
    private Vector2 dashDirection;

    private struct DashData
    {
        public float dashSpeed;
        public float dashDuration;
    }
    private DashData MainDashData;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        isDashing = false;
        MainDashData.dashSpeed = dashSpeed;
        MainDashData.dashDuration = dashDuration;
    }

    void Update()
    {
        Dash();
    }

    public void TryDash(Vector2 direction)
    {
        if(!isDashing)
        {
            isDashing = true;
            dashTimer = Time.time +dashDuration;
            dashDirection = direction;
        }
    }

    private void Dash()
    {
        if(!isDashing)
        {
            return;
        }
    
        if(Time.time < dashTimer)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else
        {
            StopDash();
        }
    }

    public void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        dashSpeed = MainDashData.dashSpeed;
        dashDuration = MainDashData.dashDuration;
    }

    public void TryTemporaryDash(Vector2 direction, float newSpeed, float newDuration)
    {
        if(!isDashing)
        {
            dashSpeed = newSpeed;
            dashDuration = newDuration;
            TryDash(direction);
        }
    }

    public void SetDashSpeed(float newSpeed)
    {
        dashSpeed = newSpeed;
    }

    public void SetDashDuration(float newDuration)
    {
        dashDuration = newDuration;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    public Vector2 GetDashDirection()
    {
        return dashDirection;
    }

    public float GetDashSpeed()
    {
        return dashSpeed;
    }

    public float GetDashDuration()
    {
        return dashDuration;
    }


}
