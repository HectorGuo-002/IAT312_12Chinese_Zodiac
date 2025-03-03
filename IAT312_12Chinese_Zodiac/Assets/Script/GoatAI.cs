using UnityEngine;

public class GoatAI : MonoBehaviour
{
    public float patrolSpeed = 2f;  // 羊的巡邏速度
    public float directionChangeInterval = 3f; // 每隔3秒改變方向
    private float directionChangeTimer;

    public int damage = 5; // 羊撞擊玩家造成的傷害
    public float knockbackForce = 5f; // 擊退力度
    public float knockbackYMultiplier = 1.5f; // Y 軸擊退倍率

    private Rigidbody2D rb;
    private bool movingRight = true; // 控制移動方向
    private SpriteRenderer spriteRenderer; // 控制圖像翻轉
    private bool isRunning = true; // 默认奔跑
    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim.SetBool("isRunning", isRunning); // ✅ 确保一开始播放 Run 动画

        if (rb == null)
            Debug.LogError("❌ Rigidbody2D 未找到！請確保 Goat 物件上有 Rigidbody2D 組件！");
        if (spriteRenderer == null)
            Debug.LogError("❌ SpriteRenderer 未找到！請確保 Goat 物件上有 SpriteRenderer 組件！");
        if (anim == null)
            Debug.LogError("❌ Animator 未找到！請確保 Goat 物件上有 Animator 組件！");

        directionChangeTimer = directionChangeInterval; // 初始化計時器
    }

    void Update()
    {
        Patrol();
        if (anim != null) // ✅ 防止 NullReferenceException
        {
            if (!anim.GetBool("isRunning"))
            {
                anim.SetBool("isRunning", true);
            }
        }
    }

    void Patrol()
    {
        // 設定移動速度
        rb.linearVelocity = new Vector2(movingRight ? patrolSpeed : -patrolSpeed, rb.linearVelocity.y);

        // 計時器倒數
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            Flip();
            directionChangeTimer = directionChangeInterval; // 重置計時器
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        
        // 確保羊的圖片方向與移動方向一致
        spriteRenderer.flipX = !movingRight; 

        Debug.Log($"🐑 Goat 方向翻轉: {(movingRight ? "向右" : "向左")}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("💥 玩家與羊發生碰撞！");

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"💥 玩家受到 {damage} 點傷害！");
            }

            if (playerRb != null)
            {
                // 禁用玩家控制，避免擊飛時還能移動
                if (playerController != null)
                {
                    playerController.enabled = false;
                }

                // 設置擊飛方向
                float direction = movingRight ? 1f : -1f;
                Vector2 knockback = new Vector2(direction * knockbackForce, knockbackForce * knockbackYMultiplier);
                playerRb.linearVelocity = knockback; // 施加擊飛力

                Debug.Log($"💨 玩家被擊飛，方向: {knockback}");

                // 延遲恢復玩家控制
                StartCoroutine(EnablePlayerControl(playerController, 0.5f));
            }
        }
    }

    System.Collections.IEnumerator EnablePlayerControl(PlayerController playerController, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}
