using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class Tutorial : MonoBehaviour
{
    private PlayerController player; // ✅ 取得玩家腳本
    private Light2D globalLight; // ✅ 取得全局燈光

    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerController>();  // ✅ 確保找到第一個 PlayerController
        globalLight = Object.FindFirstObjectByType<Light2D>();  // ✅ 找到第一個 Global Light 2D

    }

    void Update()
    {
        // ✅ 確保 `player` 和 `globalLight` 存在
        if (player != null && globalLight != null)
        {
            // ✅ 使用 `GetNightVisionCooldown()` 來讀取冷卻時間
            if (player.GetPauseTimeCooldown() <= 0 && globalLight.intensity != 1)
            {
                globalLight.intensity = 1; // ✅ 恢復光亮度
            }
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("StartMenu"); // ✅ 返回主菜单
    }
}