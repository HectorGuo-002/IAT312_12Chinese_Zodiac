using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f; // ✅ 确保游戏时间恢复正常
        SceneManager.LoadScene("LevelSelect"); // 切换到关卡选择页面
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene("Tutorial"); // 进入教程页面
    }

    public void OpenPrologue()
    {
        SceneManager.LoadScene("Prologue"); // 进入序章页面
    }

    public void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}