using UnityEngine;

public class RuneManager : MonoBehaviour
{
    public static RuneManager instance;
    private int collectedRunes = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ✅ 確保 RuneManager 不被刪除
            LoadRuneData(); // ✅ 讀取符文數據
        }
        else
        {
            Debug.LogWarning("⚠️ RuneManager 已經存在，刪除此物件");
            Destroy(gameObject); // ✅ 避免重複創建
        }
    }

    private void LoadRuneData()
    {
        collectedRunes = PlayerPrefs.GetInt("CollectedRunes", 0);
        Debug.Log($"🔄 讀取符文數據：{collectedRunes} 個符文");
    }

    public void CollectRune()
    {
        collectedRunes++;
        PlayerPrefs.SetInt("CollectedRunes", collectedRunes);
        PlayerPrefs.Save();
        Debug.Log($"🟢 已收集 {collectedRunes} 個符文");
    }

    public int GetCollectedRunes()
    {
        return collectedRunes;
    }
    void OnApplicationQuit()
    {
        Debug.Log("🛑 遊戲關閉，清除存檔數據！");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}