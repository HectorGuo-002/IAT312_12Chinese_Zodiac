using UnityEngine;

public class RuneManager : MonoBehaviour
{
    public static RuneManager instance;
    private int collectedRunes = 0; // ✅ 已收集的符文數量

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ✅ 確保場景切換時不會刪除
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectRune()
    {
        collectedRunes++;
        Debug.Log($"🟢 已收集 {collectedRunes} 個符文");

        if (collectedRunes >= 4)
        {
            Debug.Log("✅ 符文收集完畢！解鎖 Boss 關卡！");
        }
    }

    public int GetCollectedRunes()
    {
        return collectedRunes;
    }
}