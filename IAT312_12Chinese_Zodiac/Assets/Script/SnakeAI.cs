using UnityEngine;

public class SnakeAI : MonoBehaviour
{
    public float patrolSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public int damageToPlayer = 5; // 🩸 **每次傷害**
    public float damageInterval = 0.5f; // ⏳ **傷害間隔（秒）**
    public float speedReductionFactor = 2.5f; // 🐌 **降低玩家速度的比例**
    public float chaseSpeed = 4f; // 🏃 **追擊速度**
    public float detectionRange = 5f; // 🔍 **偵測玩家的距離**
    
    public float flipCooldownTime = 1f;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private float nextFlipTime = 0f;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private bool isAttached = false; // ✅ **是否附著在玩家身上**
    private PlayerController player; // ✅ **儲存玩家引用**
    private float nextDamageTime = 0f; // ⏳ **計算下一次扣血時間**
    private float originalPlayerSpeed; // ✅ **記錄玩家原始速度**

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 🔍 **開始時尋找玩家**
        InvokeRepeating("FindPlayer", 0.1f, 0.5f);
    }

    void Update()
    {
        if (!isDead && !isAttached)
        {
            FindPlayer(); // 🔍 **不斷偵測玩家**
        }
    }
    void FindPlayer()
    {
        if (isAttached || isDead) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            float distance = Vector2.Distance(transform.position, playerObj.transform.position);
            if (distance <= detectionRange) // 🔍 **如果玩家在範圍內，就追擊**
            {
                ChasePlayer(playerObj.transform);
                return;
            }
        }

        Patrol(); // 🚶‍♂️ **如果沒偵測到玩家，就執行原本的巡邏**
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
    
    void ChasePlayer(Transform playerTransform)
    {
        float direction = (playerTransform.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
        spriteRenderer.flipX = (direction > 0);
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
        if (collision.CompareTag("Player") && !isAttached)
        {
            player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                isAttached = true; // ✅ **標記已附著**
                originalPlayerSpeed = player.speed; // ✅ **記錄玩家速度**
                player.speed *= speedReductionFactor; // 🐌 **降低玩家速度**
                rb.linearVelocity = Vector2.zero; // ✅ **停止蛇的移動**
                rb.simulated = false; // ✅ **禁用物理碰撞**
                transform.SetParent(player.transform); // ✅ **讓蛇跟隨玩家**
                Debug.Log("🐍 蛇附著在玩家身上！降低移動速度！");
            }
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (isAttached && collision.CompareTag("Player"))
            {
                PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

                if (playerHealth != null && Time.time >= nextDamageTime)
                {
                    playerHealth.TakeDamage(damageToPlayer);
                    nextDamageTime = Time.time + damageInterval; // ⏳ **設定下次傷害時間**
                    Debug.Log($"💥 蛇持續傷害玩家！HP -{damageToPlayer}");
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
        {
            if (isAttached && collision.CompareTag("Player"))
            {
                Debug.Log("🐍 蛇從玩家身上脫離！");
                isAttached = false; // ✅ **標記為未附著**
                rb.simulated = true; // ✅ **重新啟用物理**
                transform.SetParent(null); // ✅ **讓蛇不再跟隨玩家**
                player.speed = originalPlayerSpeed; // 🔄 **恢復玩家速度**
                player = null; // 清除引用
            }
        }
}
