using UnityEngine;

public class BossAI : MonoBehaviour
{
    public float retreatSpeed = 3f;  // ✅ Boss 撤退速度
    public float retreatDistance = 10000f;  // ✅ 與玩家的最小距離
    public float boundaryRight = 7f;  // ✅ Boss 固定在右側的邊界
    public float verticalMoveSpeed = 2f; // ✅ 上下移動速度
    public float verticalMoveRange = 3f; // ✅ 上下移動範圍
    public float fireCooldown = 1.5f; // ✅ 發射元氣彈冷卻時間
    public float energyBallSpeed = 8f; // ✅ 元氣彈速度

    public Transform firePoint; // ✅ 發射點
    public GameObject energyBallPrefab; // ✅ 元氣彈預製體

    private Transform player;
    private float nextFireTime = 0f;
    private float initialY;
    private bool movingUp = true; // ✅ 控制 Boss 上下移動方向
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // ✅ 控制 Boss 翻轉朝向
    public bool isPaused = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // ✅ 取得 SpriteRenderer
        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody2D 未找到！請確保 FinalBoss_0 物件有 Rigidbody2D！");
            return;
        }

        rb.gravityScale = 0f; // ✅ 讓 Boss 懸空移動
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // ✅ 防止 Boss 旋轉
        initialY = transform.position.y; // ✅ 記錄 Boss 起始 Y 位置

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        else
        {
            Debug.LogError("❌ Player 未找到！請確保場景中有 Player，且 Tag 設為 Player！");
        }
    }

    void Update()
    {
        if (isPaused)
        {
            Debug.Log("👹 Boss 暫停攻擊！");
            return; // ✅ **時間暫停時不攻擊**
        }

     
        if (player == null) return;

        FlipTowardsPlayer(); // ✅ 讓 Boss 永遠面向玩家
        RetreatFromPlayer(); // ✅ 讓 Boss 遠離玩家
        MoveVertically(); // ✅ 讓 Boss 上下移動
        ShootAtPlayer(); // ✅ 讓 Boss 發射元氣彈
    }

    void RetreatFromPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < retreatDistance)
        {
            float direction = -10f; // ✅ Boss 永遠朝左撤退（固定貼在畫面右邊）

            // ✅ **確保 Boss 不會超出 boundaryRight**
            if (transform.position.x > boundaryRight)
            {
                transform.position = new Vector2(boundaryRight, transform.position.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(direction * retreatSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // ✅ 如果距離夠遠，停止撤退
        }
    }

    void MoveVertically()
    {
        // ✅ **讓 Boss 在 initialY ± verticalMoveRange 之間來回移動**
        if (movingUp)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalMoveSpeed);
            if (transform.position.y >= initialY + verticalMoveRange)
            {
                movingUp = false;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -verticalMoveSpeed);
            if (transform.position.y <= initialY - verticalMoveRange)
            {
                movingUp = true;
            }
        }
    }

    void FlipTowardsPlayer()
    {
        if (player == null) return;

        // ✅ 如果玩家在 Boss 左邊，翻轉 Boss 朝左；如果玩家在右邊，朝右
        bool shouldFaceRight = player.position.x > transform.position.x;
        spriteRenderer.flipX = shouldFaceRight; 
    }

    void ShootAtPlayer()
    {
        if (isPaused) return;
        if (Time.time > nextFireTime && energyBallPrefab != null && firePoint != null && player != null)
        {
            GameObject energyBall = Instantiate(energyBallPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = energyBall.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (player.position - firePoint.position).normalized;
                rb.linearVelocity = direction * energyBallSpeed;
            }

            Debug.Log("🔥 Boss 發射元氣彈！");
            nextFireTime = Time.time + fireCooldown;
        }
    }
    
    public void ResumeAfterPause()
    {
        isPaused = false;
        nextFireTime = Time.time; // ✅ **立即恢復攻擊冷卻**
        Debug.Log("👹 Boss 恢復攻擊！");
        ShootAtPlayer(); // ✅ **立即嘗試攻擊**
    }
}