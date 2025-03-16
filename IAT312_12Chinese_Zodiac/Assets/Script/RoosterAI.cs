using UnityEngine;
using System.Collections;

public class RoosterAI : MonoBehaviour
{
    public float patrolSpeed = 2f;  
    public float detectionRange = 7f;  
    public float fireCooldown = 0.2f; 
    public float energyBallSpeed = 10f; 
    public Transform groundCheck;  
    public LayerMask groundLayer;  
    public GameObject energyBallPrefab; 
    public Transform firePoint;  
    public float flipCooldownTime = 1f; 

    private Rigidbody2D rb;
    private Transform player;
    private float nextFireTime = 0f; 
    private bool movingRight = true; 
    private float nextFlipTime = 0f; 
    private SpriteRenderer spriteRenderer;
    public bool isPaused = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
            Debug.LogError("❌ Rigidbody2D 未找到！請確保 Rooster 物件上有 Rigidbody2D 組件！");
        if (spriteRenderer == null)
            Debug.LogError("❌ SpriteRenderer 未找到！請確保 Rooster 物件上有 SpriteRenderer 組件！");

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        else
        {
            Debug.LogError("❌ Player 未找到！請確保場景中有 Player，且 Tag 設為 Player！");
        }

        nextFireTime = Time.time + fireCooldown;
    }

    void Update()
    {
        if (isPaused)
        {
            rb.linearVelocity = Vector2.zero; // ✅ 確保速度為 0
            rb.simulated = false; // ✅ **完全凍結物理運算**
            return; // ✅ **跳過 Update 剩餘部分，確保 Rooster 不會動作**
        }
        else
        {
            rb.simulated = true; // ✅ **時間恢復後重新啟用物理**
        }

        if (player == null) return;

        // Patrol(); ✅ **讓雞在平台上來回移動**
        ShootAtPlayer(); // ✅ **讓雞開始攻擊**
    }

    bool IsGroundAhead()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        return groundInfo.collider != null;
    }

    void Flip()
    {
        if (isPaused) return; // ✅ **時間暫停時不翻轉**
        
        movingRight = !movingRight;
        spriteRenderer.flipX = movingRight;
        Debug.Log($"🐔 Rooster 方向翻轉: {(movingRight ? "向右" : "向左")}");
    }

    void ShootAtPlayer()
    {
        if (isPaused) return; // ✅ **時間暫停時不開火**
        
        if (Time.time > nextFireTime && player != null)
        {
            FireEnergyBall();
            nextFireTime = Time.time + fireCooldown; 
        }
    }

    void FireEnergyBall()
    {
        if (isPaused) return; // ✅ **時間暫停時不發射子彈**
        if (energyBallPrefab == null || firePoint == null || player == null) return;

        GameObject energyBall = Instantiate(energyBallPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = energyBall.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            rb.linearVelocity = direction * energyBallSpeed;
        }

        Debug.Log($"🐔 Rooster 發射了元氣彈！ 方向: {rb.linearVelocity}");
    }
    public void ResumeAfterPause()
    {
        isPaused = false;
        rb.simulated = true; // ✅ 確保物理重新啟用
        nextFireTime = Time.time; // ✅ 立刻恢復攻擊，不受冷卻影響
        ShootAtPlayer(); // ✅ 直接觸發攻擊，避免等下一個 Update()
    }

} 