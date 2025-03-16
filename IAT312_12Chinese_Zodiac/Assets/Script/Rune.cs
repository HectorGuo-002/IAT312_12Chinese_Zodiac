using UnityEngine;

public class Rune : MonoBehaviour
{
    public string runeKey; // ✅ 設定此符文的唯一標識 (例如 "GoatRune", "RoosterRune")
    public GameObject levelCompleteUI; // ✅ 過關 UI 物件

    void Start()
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false); // ✅ 確保 UI 預設是關閉的
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"🎉 玩家收集了符文: {runeKey}");

            // ✅ 記錄符文已收集
            PlayerPrefs.SetInt(runeKey, 1);
            PlayerPrefs.SetInt("CollectedRunes", PlayerPrefs.GetInt("CollectedRunes", 0) + 1);
            PlayerPrefs.Save();

            Debug.Log($"💾 PlayerPrefs 存儲檢查: {runeKey} = {PlayerPrefs.GetInt(runeKey, 0)}");
            Debug.Log($"💾 符文數量存儲檢查: {PlayerPrefs.GetInt("CollectedRunes", 0)}");

            // ✅ 顯示過關 UI
            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
                Time.timeScale = 0f; // ✅ 暫停遊戲
            }

            // ✅ 隱藏符文
            gameObject.SetActive(false);
        }
    }
}