using UnityEngine;
using System.Collections;

public class InkProjectile : MonoBehaviour
{
    public float speed = 10f; // 子弹速度
    public float lifetime = 3f; // 存活时间
    public int damage = 10; // 伤害值
    public LayerMask groundLayers;

    private Rigidbody2D rb;

    void Start()
    {
        Debug.Log("🚀 墨汁子弹生成！");
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody2D 未找到！");
            return;
        }

        // **🔥 获取 FirePoint 父对象的方向**
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform != null)
        {
            float direction = playerTransform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * speed, 0);

            // ✅ **改用 SpriteRenderer.flipX**
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.flipX = (direction < 0); // 🔄 **當朝向左時，翻轉圖片**
            }
        }
        else
        {
            Debug.LogError("❌ 找不到 Player！");
        }

        // **3 秒后自动销毁**
        StartCoroutine(AutoDestroy());
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        Debug.Log("🔥 墨汁子弹消失！");
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"💥 墨汁碰撞到了 {collision.gameObject.name}");
        if (collision.CompareTag("Boss")) // ✅ 確保 Boss 也會受傷
        {
            BossHealth bossHealth = collision.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
                Debug.Log($"⚔️ 墨汁擊中了 Boss！造成 {damage} 傷害！");
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>(); // **改成 `EnemyHealth`**
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"💥 墨汁击中了敌人！造成 {damage} 伤害！");
            }
            else
            {
                Debug.LogError("❌ 未找到 `EnemyHealth` 组件！");
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Gong")) // ✅ 確保鑼鼓有 Tag "Gong"
        {
            Destroy(gameObject); // ✅ 墨汁擊中鑼鼓後消失
        }
        else if (((1 << collision.gameObject.layer) & groundLayers) != 0) // ✅ **檢測 Layer 6~9**
        {
            Debug.Log("🧱 墨汁击中了地面 (Layer 6~9)，销毁！");
            Destroy(gameObject);
        }
    }
}