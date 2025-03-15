using UnityEngine;
using System.Collections;

public class RoosterAI : MonoBehaviour
{
    public float patrolSpeed = 2f;  // ✅ 雞的移動速度
    public float detectionRange = 7f;  // ✅ 偵測玩家範圍
    public float fireCooldown = 0.2f; // ✅ 降低冷卻時間（更快射擊）
    public float energyBallSpeed = 10f; // ✅ 增加元氣彈速度
    public Transform groundCheck;   // ✅ 地面檢測
    public LayerMask groundLayer;   // ✅ 地面圖層
    public GameObject energyBallPrefab; // ✅ 元氣彈預製體
    public Transform firePoint;  // ✅ 發射點
    public float flipCooldownTime = 1f; // ✅ 增加翻轉冷卻時間，避免瘋狂翻轉

    private Rigidbody2D rb;
    private Transform player;
    private float nextFireTime = 0f; // ✅ 控制射擊冷卻時間
    private bool movingRight = true; // ✅ 控制雞的移動方向
    private float nextFlipTime = 0f; // ✅ 翻轉方向冷卻時間
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
            Debug.LogError("❌ Rigidbody2D 未找到！請確保 `Rooster` 物件上有 Rigidbody2D 組件！");
        if (spriteRenderer == null)
            Debug.LogError("❌ SpriteRenderer 未找到！請確保 `Rooster` 物件上有 SpriteRenderer 組件！");

        // ✅ 找到玩家
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        else
        {
            Debug.LogError("❌ `Player` 未找到！請確保場景中有 `Player`，且 `Tag` 設為 `Player`！");
        }

        // ✅ 遊戲開始就發射元氣彈
        nextFireTime = Time.time + fireCooldown;
    }

    void Update()
    {
        if (player == null) return; // ✅ 避免 `player` 為 `null` 時拋出錯誤

        // Patrol(); ✅ 讓雞在平台上來回移動
        ShootAtPlayer(); // ✅ 讓雞從一開始就開始發射元氣彈
    }

    // void Patrol()
    // {
    //     // ✅ 讓雞在平台上左右來回移動
    //     rb.linearVelocity = new Vector2(movingRight ? patrolSpeed : -patrolSpeed, rb.linearVelocity.y);
    //
    //     // ✅ 檢測前方是否還有地面，否則回頭
    //     if (!IsGroundAhead() && Time.time > nextFlipTime)
    //     {
    //         Flip();
    //         nextFlipTime = Time.time + flipCooldownTime; // ✅ 設定翻轉冷卻時間，避免瘋狂翻轉
    //     }
    // }

    bool IsGroundAhead()
    {
        // ✅ **正確的地面檢測方式，向下發射射線**
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        return groundInfo.collider != null;
    }

    void Flip()
    {
        movingRight = !movingRight;
        spriteRenderer.flipX = movingRight; // ✅ **修正翻轉方式，確保圖片方向正確**
        Debug.Log($"🐔 Rooster 方向翻轉: {(movingRight ? "向右" : "向左")}");
    }

    void ShootAtPlayer()
    {
        if (Time.time > nextFireTime && player != null)
        {
            FireEnergyBall();
            nextFireTime = Time.time + fireCooldown; // ✅ 設定冷卻時間，確保雞一直發射
        }
    }

    void FireEnergyBall()
    {
        if (energyBallPrefab == null || firePoint == null || player == null) return;

        GameObject energyBall = Instantiate(energyBallPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = energyBall.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // ✅ **確保元氣彈朝玩家的即時位置發射**
            Vector2 direction = (player.position - firePoint.position).normalized;
            rb.linearVelocity = direction * energyBallSpeed; // ✅ 修正 `linearVelocity` → `velocity`
        }

        Debug.Log($"🐔 Rooster 發射了元氣彈！ 方向: {rb.linearVelocity}");
    }
}
