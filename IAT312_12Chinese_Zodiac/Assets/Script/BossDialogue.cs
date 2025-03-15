using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossDialogue : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject dialogueBox; // ✅ 對話框
    public TextMeshProUGUI npcNameText; // ✅ NPC 名稱
    public GameObject playerPortraitObject; // ✅ 玩家頭像 (GameObject)
    public GameObject npcPortraitObject; // ✅ NPC 頭像 (GameObject)
    public TextMeshProUGUI dialogueText; // ✅ 顯示對話內容

    [Header("選擇按鈕 (預設隱藏)")]
    public GameObject choicePanel; // ✅ **選擇按鈕面板 (預設關閉)**
    public Button forgiveButton; // ✅ **原諒按鈕**
    public Button killButton; // ✅ **擊殺按鈕**

    [Header("對話內容")]
    public BossDialogueLine[] dialogueLines; // ✅ **存放 BOSS 對話內容**
    private int currentLine = 0; // ✅ **當前對話索引**
    private bool isDialogueActive = false; // ✅ **是否正在對話**
    public float textSpeed = 0.05f; // ✅ **文字顯示速度**
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private Image playerPortraitImage; // ✅ 玩家頭像圖片
    private Image npcPortraitImage; // ✅ NPC 頭像圖片

    [Header("結局場景")]
    public string ending1Scene; // ✅ **選擇 1 的結局場景**
    public string ending2Scene; // ✅ **選擇 2 的結局場景**

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

        if (npcPortraitImage == null)
        {
            Debug.LogError("❌ `NpcPortrait` 缺少 `Image` 組件！");
        }

        dialogueBox.SetActive(false);
        choicePanel.SetActive(false); // ✅ **預設隱藏選擇按鈕**
        
        forgiveButton.onClick.AddListener(ChooseForgive);
        killButton.onClick.AddListener(ChooseKill);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isDialogueActive)
        {
            if (isTyping)
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

    public void StartDialogue()
    {
        currentLine = 0;
        isDialogueActive = true;
        dialogueBox.SetActive(true);
        choicePanel.SetActive(false); // ✅ **先確保選擇按鈕是關閉的**
        Time.timeScale = 0f; // ✅ **暫停遊戲，確保 BOSS 不會攻擊**
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentLine >= dialogueLines.Length)
        {
            StartCoroutine(ShowChoicesWithDelay()); // ✅ **對話完畢後，顯示選擇按鈕**
            return;
        }

        npcNameText.text = dialogueLines[currentLine].npcName;

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

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
        isTyping = false;
    }

    IEnumerator ShowChoicesWithDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f); // ✅ **讓玩家有短暫時間思考**
        EndDialogue();
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
        choicePanel.SetActive(true); // ✅ **對話結束後顯示選擇按鈕**
    }

    public void ChooseForgive()
    {
        StartCoroutine(TransitionToScene(ending1Scene));
    }

    public void ChooseKill()
    {
        StartCoroutine(TransitionToScene(ending2Scene));
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        choicePanel.SetActive(false);
        dialogueText.text = "Loading Ending...";
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f; // ✅ **恢復遊戲**
        SceneManager.LoadScene(sceneName);
    }
}

// ✅ **定義對話結構**
[System.Serializable]
public class BossDialogueLine
{
    public string npcName; // **對話角色名稱**
    public Sprite npcPortrait; // **對話角色頭像**
    public bool isPlayerSpeaking; // **是否為玩家發言**
    [TextArea(2, 5)]
    public string dialogue; // **對話內容**
}
