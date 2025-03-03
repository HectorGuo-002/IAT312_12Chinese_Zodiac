using UnityEngine;

public class DragonAI : MonoBehaviour
{
    [Header("移動設定")]
    public float speed = 3f; // 龍的移動速度
    public Transform groundCheck; // 用於偵測地面邊界
    public LayerMask groundLayer; // 設定地面圖層
    private bool movingRight = true; // 是否朝右移動
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("攻擊設定")]
    public int damage = 10; // 碰到玩家時造成的傷害

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (groundCheck == null)
        {
            Debug.LogError("❌ groundCheck 未綁定！請在 Inspector 設置！");
        }
    }

    void Update()
    {
        Move();
        CheckEdge();
    }

    /// <summary>
    /// 讓龍左右來回移動
    /// </summary>
    void Move()
    {
        rb.linearVelocity = new Vector2(movingRight ? speed : -speed, rb.linearVelocity.y);
    }


    void CheckEdge()
    {
        bool isGroundAhead = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (!isGroundAhead) // 如果前方沒有地面
        {
            Flip();
        }
    }


    void Flip()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}