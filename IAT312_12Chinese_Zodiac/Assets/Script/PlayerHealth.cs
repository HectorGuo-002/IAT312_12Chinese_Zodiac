using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject gameOverUI; // 玩家死亡時的 UI
    public Slider healthSlider;
    public TMP_Text healthText;
    public int regenAmount = 5; // ❤️ **每次回血量**
    public float regenInterval = 3f; // ⏳ **回血間隔（秒）**
    private float lastDamageTime = 0f; // ⏳ **紀錄最近受傷的時間**

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth; // 初始同步血條
        }
        gameOverUI.SetActive(false); // 確保遊戲開始時 UI 隱藏
        StartCoroutine(AutoRegenHealth());
        
        IEnumerator AutoRegenHealth()
        {
            while (true)
            {
                yield return new WaitForSeconds(regenInterval);

                // **受傷後 5 秒內不回血**
                if (Time.time - lastDamageTime >= 5f && currentHealth > 0 && currentHealth < maxHealth)
                {
                    currentHealth = Mathf.Min(currentHealth + regenAmount, maxHealth);
                    UpdateHealthUI();
                    Debug.Log($"💖 自動回血：+{regenAmount} HP，當前血量：{currentHealth}/{maxHealth}");
                }
            }
        }
    }
    

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 確保血量不低於 0
        lastDamageTime = Time.time;

        Debug.Log($"🔥 敵人受到 {damage} 傷害！剩餘血量：{currentHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💀 玩家死亡！");
        Time.timeScale = 0f; // 暫停遊戲
        gameOverUI.SetActive(true);
    }

    public void Respawn()
    {
        Time.timeScale = 1f; // 恢復遊戲
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 重新載入關卡
    }

    public void ReturnToLevelSelect()
    {
        Time.timeScale = 1f; // 恢復遊戲
        SceneManager.LoadScene("LevelSelect");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spike")) // ✅ 確保地刺的 Tag 設為 "Spike"
        {
            Debug.Log("☠ 玩家觸碰到地刺，直接死亡！");
            Die(); // 直接執行死亡方法
        }
    }
    
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}