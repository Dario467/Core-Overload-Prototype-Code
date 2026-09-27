using UnityEngine;

public class Mononch : MonoBehaviour, IWander
{
    enemyAi ai;
    Rigidbody2D rb;
    [SerializeField] private float timeToTurn;
    private float turnTimer;

    private bool startWandering;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ai = GetComponent<enemyAi>();
    }

    void Start()
    {
        startWandering = false;
    }

    public void ExecuteWander()
    {
        if(!startWandering)
        {
            turnTimer = Time.time + timeToTurn;
            startWandering = true;
        }
        rb.linearVelocity = new Vector2 (ai.GetMovingDirection().x*ai.GetSpeed(), rb.linearVelocity.y);
        if(ai.IsWallAhead() || Time.time >= turnTimer)
        {
            startWandering = false;
            ai.ChangeCurrentState(enemyAi.State.turn);
        }
    }
}
