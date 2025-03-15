using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class BossHealth : MonoBehaviour
{
    [Header("BOSS 屬性")]
    public int maxHealth = 100;
    private int currentHealth;
    public BossDialogue bossDialogue; // ✅ **新增 BOSS 劇情管理器**
    [Header("BOSS 屬性")]
    public Slider healthSlider;

    private bool hasTriggeredDialogue = false; // ✅ **防止重複觸發對話**

    void Start()
    {
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
}