using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject dialogueBox; // ✅ 對話框
    public TextMeshProUGUI npcNameText; // ✅ 顯示 NPC 名稱
    public GameObject playerPortraitObject; // ✅ 玩家頭像 (GameObject)
    public GameObject npcPortraitObject; // ✅ NPC 頭像 (GameObject)
    public TextMeshProUGUI dialogueText; // ✅ 顯示對話內容

    [Header("對話設定")]
    public DialogueLine[] dialogueLines; // ✅ 存放所有對話
    private int currentLine = 0; // ✅ 當前對話索引
    private bool isDialogueActive = false; // ✅ 是否正在對話
    public float inputCooldown = 0.5f; // ✅ 防止 `E` 連續觸發
    private float lastInputTime = -1f; // ✅ 記錄 `E` 的最後輸入時間
    public string levelToLoad; // ✅ 對話結束後要載入的場景

    [Header("文字速度設定")]
    public float textSpeed = 0.05f; // ✅ 控制文字顯示速度
    private Coroutine typingCoroutine; // ✅ 控制打字動畫
    private bool isTyping = false; // ✅ 避免玩家跳過對話時出錯

    private Image playerPortraitImage; // ✅ 玩家頭像圖片
    private Image npcPortraitImage; // ✅ NPC 頭像圖片

    void Start()
    {
        if (dialogueBox == null || npcNameText == null || playerPortraitObject == null || npcPortraitObject == null || dialogueText == null)
        {
            Debug.LogError("❌ UI 元件未綁定！請檢查 Inspector 設置！");
            return;
        }

        // ✅ 確保 `GameObject` 內有 `Image` 組件
        playerPortraitImage = playerPortraitObject.GetComponent<Image>();
        npcPortraitImage = npcPortraitObject.GetComponent<Image>();

        if (playerPortraitImage == null)
        {
            Debug.LogError("❌ `PlayerPortrait` 缺少 `Image` 組件！");
        }
        else
        {
            Debug.Log("✅ `PlayerPortrait` 綁定成功：" + playerPortraitImage);
        }

        if (npcPortraitImage == null)
        {
            Debug.LogError("❌ `NpcPortrait` 缺少 `Image` 組件！");
        }
        else
        {
            Debug.Log("✅ `NpcPortrait` 綁定成功：" + npcPortraitImage);
        }

        dialogueBox.SetActive(false); // ✅ 預設關閉對話框
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Time.time - lastInputTime >= inputCooldown && isDialogueActive)
        {
            lastInputTime = Time.time;

            if (isTyping) // ✅ 如果還在顯示文字，則跳過打字動畫
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueLines[currentLine].dialogue;
                isTyping = false;
            }
            else
            {
                currentLine++;
                DisplayNextLine();
            }
        }
    }

    /// <summary>
    /// 啟動對話
    /// </summary>
    public void StartDialogue()
    {
        if (dialogueLines.Length == 0)
        {
            Debug.LogError("❌ `DialogueLines` 沒有任何對話內容！");
            return;
        }

        currentLine = 0;
        isDialogueActive = true;
        dialogueBox.SetActive(true);
        DisplayNextLine();
        Debug.Log("▶️ 對話開始");
    }

    /// <summary>
    /// 顯示下一行對話（逐字顯示）
    /// </summary>
    public void DisplayNextLine()
    {
        if (!isDialogueActive) return;

        if (currentLine >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        npcNameText.text = dialogueLines[currentLine].npcName;

        // ✅ 確保 `Sprite` 存在
        if (dialogueLines[currentLine].npcPortrait == null)
        {
            Debug.LogError($"❌ `npcPortrait` 為 null，請檢查對話資料 (Line: {currentLine})");
        }
        else
        {
            Debug.Log($"✅ `npcPortrait` 成功取得：{dialogueLines[currentLine].npcPortrait.name}");
        }

        // ✅ 顯示正確的頭像
        if (dialogueLines[currentLine].isPlayerSpeaking)
        {
            playerPortraitObject.SetActive(true);
            npcPortraitObject.SetActive(false);

            if (playerPortraitImage != null)
            {
                playerPortraitImage.sprite = dialogueLines[currentLine].npcPortrait;
            }
        }
        else
        {
            playerPortraitObject.SetActive(false);
            npcPortraitObject.SetActive(true);

            if (npcPortraitImage != null)
            {
                npcPortraitImage.sprite = dialogueLines[currentLine].npcPortrait;
            }
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeSentence(dialogueLines[currentLine].dialogue));
    }

    /// <summary>
    /// 逐字顯示對話
    /// </summary>
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    /// <summary>
    /// 結束對話
    /// </summary>
    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);

        if (!string.IsNullOrEmpty(levelToLoad))
        {
            Debug.Log("🚀 載入場景：" + levelToLoad);
            SceneManager.LoadScene(levelToLoad);
        }
    }
    public bool IsDialogueFinished()
    {
        return currentLine >= dialogueLines.Length;
    }
    public void ForceCloseDialogue()
    {
        dialogueBox.SetActive(false);
    }
}

// ✅ **定義對話結構**
[System.Serializable]
public class DialogueLine
{
    public string npcName; // NPC 名稱
    public Sprite npcPortrait; // 頭像（玩家或 NPC）
    public bool isPlayerSpeaking; // ✅ 判斷「誰說話」
    [TextArea(2, 5)]
    public string dialogue; // 對話內容
}
