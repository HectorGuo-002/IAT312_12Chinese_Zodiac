using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class BossHealth : MonoBehaviour
{
    [Header("BOSS 屬性")]
    public int baseHealth = 100;
    private int maxHealth;
    private int currentHealth;
    public BossDialogue bossDialogue; // ✅ **新增 BOSS 劇情管理器**
    [Header("BOSS 屬性")]
    public Slider healthSlider;

    private bool hasTriggeredDialogue = false; // ✅ **防止重複觸發對話**

    void Start()
    {
        int runeCount = GetTotalRunes(); // 🔍 **計算玩家擁有的符文數**
        maxHealth = baseHealth + (runeCount * 50); // 🩸 **每個符文 +50 血量**
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (hasTriggeredDialogue) return; // ✅ **正在進入劇情時，不再受傷害**

        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 10 && !hasTriggeredDialogue)
        {
            TriggerDialogue();
        }

        if (currentHealth <= 0 && !hasTriggeredDialogue)
        {
            currentHealth = 1; // ✅ **防止 BOSS 直接死亡**
            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        hasTriggeredDialogue = true;
        bossDialogue.StartDialogue(); // ✅ **通知 `BossDialogue` 啟動劇情**
    }
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
    int GetTotalRunes()
    {
        int goatRune = PlayerPrefs.GetInt("GoatRune", 0);
        int snakeRune = PlayerPrefs.GetInt("SnakeRune", 0);
        int roosterRune = PlayerPrefs.GetInt("RoosterRune", 0);
        int dragonRune = PlayerPrefs.GetInt("DragonRune", 0);

        int totalRunes = goatRune + snakeRune + roosterRune + dragonRune;
        Debug.Log("📜 玩家總共獲得 " + totalRunes + " 個符文");

        return totalRunes;
    }
}