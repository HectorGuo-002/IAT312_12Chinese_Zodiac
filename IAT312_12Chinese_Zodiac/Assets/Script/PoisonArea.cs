using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    public int damage = 1; // ✅ 每秒伤害
    public float damageInterval = 1f; // ✅ 伤害间隔时间
    private bool playerInside = false;
    private float nextDamageTime = 0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        if (playerInside && Time.time >= nextDamageTime)
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>(); // ✅ 获取玩家生命值组件
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            nextDamageTime = Time.time + damageInterval; // ✅ 设置下一次伤害时间
        }
    }
}