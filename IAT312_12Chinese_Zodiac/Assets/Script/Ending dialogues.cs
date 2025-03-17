using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingDialogues : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject dialogueBox; // ✅ 对话框
    public TextMeshProUGUI npcNameText; // ✅ NPC 名称
    public GameObject playerPortraitObject; // ✅ 玩家头像
    public GameObject npcPortraitObject; // ✅ NPC 头像
    public TextMeshProUGUI dialogueText; // ✅ 对话内容

    [Header("选择按钮 (默认隐藏)")]
    public GameObject choicePanel; // ✅ **选择按钮面板**
    public Button forgiveButton; // ✅ **原谅按钮**
    public Button killButton; // ✅ **击杀按钮**

    [Header("对话内容")]
    public BossDialogueLine[] dialogueLines; // ✅ **存放对话内容**
    private int currentLine = 0; // ✅ **当前对话索引**
    private bool isDialogueActive = false; // ✅ **是否正在对话**
    public float textSpeed = 0.05f; // ✅ **文字显示速度**
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private Image playerPortraitImage; // ✅ 玩家头像图片
    private Image npcPortraitImage; // ✅ NPC 头像图片

    [Header("结局场景")]
    public string ending1Scene; // ✅ **选项 1 结局**
    public string ending2Scene; // ✅ **选项 2 结局**

    void Start()
    {
        // ✅ 确保 UI 组件存在
        if (dialogueBox == null || npcNameText == null || playerPortraitObject == null || npcPortraitObject == null || dialogueText == null)
        {
            Debug.LogError("❌ UI 组件未绑定！请检查 Inspector 设置！");
            return;
        }

        // ✅ 获取头像组件
        playerPortraitImage = playerPortraitObject.GetComponent<Image>();
        npcPortraitImage = npcPortraitObject.GetComponent<Image>();

        dialogueBox.SetActive(false);
        choicePanel.SetActive(false); // ✅ **默认隐藏选项按钮**

        // ✅ 绑定按钮点击事件
        forgiveButton.onClick.AddListener(ChooseForgive);
        killButton.onClick.AddListener(ChooseKill);

        // ✅ **进入场景后自动开始对话**
        StartDialogue();
    }

    void Update()
    {
        // ✅ 按 `E` 继续对话
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
        choicePanel.SetActive(false); // ✅ **确保选项按钮关闭**
        Time.timeScale = 0f; // ✅ **暂停游戏**
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentLine >= dialogueLines.Length)
        {
            StartCoroutine(ShowChoicesWithDelay()); // ✅ **对话结束后显示选择**
            return;
        }

        npcNameText.text = dialogueLines[currentLine].npcName;

        // ✅ 显示正确的头像
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
        yield return new WaitForSecondsRealtime(1.5f); // ✅ **等待 1.5 秒后显示选项**
        EndDialogue();
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
        choicePanel.SetActive(true); // ✅ **对话结束后显示选项**
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
        Time.timeScale = 1f; // ✅ **恢复游戏**
        SceneManager.LoadScene(sceneName);
    }
}



