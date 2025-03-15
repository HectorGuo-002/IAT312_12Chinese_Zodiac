using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public string runeName; // ✅ 設定符文名稱（GoatRune、RoosterRune、SnakeRune、DragonRune）

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"🟠 玩家獲得符文：{runeName}");

            if (RuneManager.instance != null)
            {
                RuneManager.instance.CollectRune();
                Destroy(gameObject); // ✅ 獲得符文後消失
            }
        }
    }
}