using UnityEngine;

public class SnakeAI : MonoBehaviour
{
    public float patrolSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public int damageToPlayer = 10;
    public float flipCooldownTime = 1f;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private float nextFlipTime = 0f;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (!isDead)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        rb.linearVelocity = new Vector2(movingRight ? patrolSpeed : -patrolSpeed, rb.linearVelocity.y);

        if ((!IsGroundAhead()) && Time.time > nextFlipTime)
        {
            Flip();
            nextFlipTime = Time.time + flipCooldownTime;
        }
    }

    bool IsGroundAhead()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        return groundInfo.collider != null;
    }

    void Flip()
    {
        movingRight = !movingRight;
        spriteRenderer.flipX = movingRight;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        isDead = true;
        Debug.Log($"🐍 Snake 受到 {damage} 點傷害，死亡！");

        Die();
    }

    void Die()
    {
        rb.linearVelocity = Vector2.zero;
        patrolSpeed = 0f;

        // **通知 PoisonCloudManager 在這個位置生成毒霧**
        PoisonCloudManager.SpawnPoisonCloud(transform.position);

        Destroy(gameObject); // **摧毀蛇**
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
                Debug.Log($"💥 玩家碰撞蛇！HP -{damageToPlayer}");
            }
        }
    }
}
