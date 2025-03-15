using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGate : MonoBehaviour
{
    public DialogueSystem dialogueSystem;
    public string levelToLoad; // ✅ **手動設定場景名稱**
    private bool playerInRange = false; // ✅ **玩家是否在門的範圍內**
    private bool hasStartedDialogue = false; // ✅ **確保對話不會重複觸發**
    private bool isDialogueCompleted = false; // ✅ **確保對話結束後才能進入關卡**
    public bool isBossGate = false;

    void Start()
    {
        if (dialogueSystem == null)
        {
            dialogueSystem = FindFirstObjectByType<DialogueSystem>();
            if (dialogueSystem == null)
            {
                Debug.LogError("❌ `DialogueSystem` 未找到！請確保場景內有 `DialogueSystem` 物件！");
                return;
            }
        }

        dialogueSystem.levelToLoad = levelToLoad; // ✅ **將 LevelGate 的目標場景傳給 DialogueSystem**
    }

    void Update()
    {
        //✅ **普通門可以直接開始對話**
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !hasStartedDialogue && !isDialogueCompleted)
        {
        
            if (isBossGate) // ✅ **如果是 BOSS 門，需要符文檢查**
            {
                if (RuneManager.instance == null)
                {
                    Debug.LogError("❌ `RuneManager` 未初始化，請確保 `RuneManager` 存在！");
                    return;
                }
            
                int runeCount = RuneManager.instance.GetCollectedRunes();
                Debug.Log($"🟢 符文數量檢查: {runeCount} / 1");
            
                if (runeCount >= 1) // ✅ **必須至少收集 1 個符文才能進入**
                {
                    hasStartedDialogue = true;
                    dialogueSystem.StartDialogue();
                }
                else
                {
                    Debug.Log("🚫 你需要至少 1 個符文才能進入最終 BOSS 戰！");
                }
            }
            else // ✅ **普通門可以直接進入**
            {
                hasStartedDialogue = true;
                dialogueSystem.StartDialogue();
            }
        }

        // ✅ **當對話結束，允許進入關卡**
        if (hasStartedDialogue && dialogueSystem.IsDialogueFinished())
        {
            isDialogueCompleted = true;
            hasStartedDialogue = false;
        }

        // ✅ **玩家必須再次按 `E` 才會進入關卡**
        if (isDialogueCompleted && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        Debug.Log($"🚀 傳送到場景：{levelToLoad}");
        SceneManager.LoadScene(levelToLoad);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true; // ✅ **玩家進入範圍**
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // ✅ **玩家離開範圍**
            CancelDialogue(); // ✅ **當玩家離開時，只關閉對話，不影響傳送條件**
        }
    }

    /// <summary>
    /// ✅ **當玩家離開範圍時，只關閉對話框，不會影響 `isDialogueCompleted`**
    /// </summary>
    void CancelDialogue()
    {
        if (hasStartedDialogue)
        {
            dialogueSystem.ForceCloseDialogue(); // ✅ **呼叫 `ForceCloseDialogue()` 來關閉對話框**
            hasStartedDialogue = false; // ✅ **標記對話未開始**
        }
    }
}
