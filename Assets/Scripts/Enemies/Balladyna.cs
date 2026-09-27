using UnityEngine;
using UnityEngine.UIElements;

public class Balladyna : MonoBehaviour, IWander
{
    enemyAi ai;
    Rigidbody2D rb;
    private EnemyHitbox hitbox;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ai = GetComponent<enemyAi>();
        hitbox = GetComponentInChildren<EnemyHitbox>();
    }

    void Start()
    {
    }

    public void ExecuteWander()
    {
        rb.linearVelocity = new Vector2 (ai.GetMovingDirection().x*ai.GetSpeed(), rb.linearVelocity.y);
        if(ai.IsWallAhead() || !ai.IsFloorAhead())
        {
            ai.ChangeCurrentState(enemyAi.State.turn);
        }
    }

    public void ActivateHitbox()
    {
        hitbox.ActivateHitbox();
    }

    public void DesactivateHitbox()
    {
        hitbox.DesactivateHitbox();
    }
}
