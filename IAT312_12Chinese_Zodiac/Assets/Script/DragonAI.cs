using UnityEngine;

public class DragonAI : MonoBehaviour
{
    [Header("飛行設定")]
    public float flySpeed = 2f; // 飛行速度
    public float flyTime = 2f; // 每次飛行的時間（向上 2 秒，向下 2 秒）

    private Rigidbody2D rb;
    private bool flyingUp = true; // 是否正在向上飛
    private float switchDirectionTime; // 切換飛行方向的時間
    private Transform player;

    [Header("攻擊設定")]
    public int damage = 10; // 碰到玩家時造成的傷害
    public float knockbackForce = 5f; // ✅ 新增擊退力度
    public GameObject shockwavePrefab; // ✅ 衝擊波預製體
    public Transform firePoint; // ✅ 發射衝擊波的起點
    public float fireCooldown = 2f; // ✅ 發射間隔時間
    private float nextFireTime = 0f; // ✅ 控制射擊冷卻時間

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // ✅ 自動找到玩家
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        else
        {
            Debug.LogError("❌ 玩家未找到！請確保場景中有 `Player`，且 `Tag` 設為 `Player`");
        }

        switchDirectionTime = Time.time + flyTime; // ✅ 設定飛行切換時間
    }

    void Update()
    {
        FlyUpDown(); // ✅ 龍會上下飛行
        ShootWave(); // ✅ 龍會發射衝擊波
    }

    /// <summary>
    /// 讓龍上下飛行
    /// </summary>
    void FlyUpDown()
    {
        // 每 `flyTime` 秒切換一次方向
        if (Time.time >= switchDirectionTime)
        {
            flyingUp = !flyingUp; // 切換方向
            switchDirectionTime = Time.time + flyTime; // 設定下一次切換時間
        }

        // ✅ 修正 `rb.linearVelocity` → `rb.velocity`
        rb.linearVelocity = new Vector2(0, flyingUp ? flySpeed : -flySpeed);
    }

    /// <summary>
    /// 當玩家碰到龍時，造成傷害並擊退
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // ✅ 修正 `playerRb.linearVelocity` → `playerRb.velocity`
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerRb.linearVelocity = knockbackDirection * knockbackForce;
            }
        }
    }

    /// <summary>
    /// 發射衝擊波
    /// </summary>
    void ShootWave()
    {
        if (Time.time >= nextFireTime && shockwavePrefab != null && firePoint != null && player != null)
        {
            GameObject shockwave = Instantiate(shockwavePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rbShockwave = shockwave.GetComponent<Rigidbody2D>();

            if (rbShockwave != null)
            {
                // ✅ 計算朝向玩家的方向
                Vector2 direction = (player.position - firePoint.position).normalized;
                
                // ✅ 修正 `rbShockwave.linearVelocity` → `rbShockwave.velocity`
                rbShockwave.linearVelocity = direction * 7f;
            }

            nextFireTime = Time.time + fireCooldown;
        }
    }
}
