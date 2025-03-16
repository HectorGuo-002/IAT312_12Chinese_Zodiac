using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("基本屬性")] public float speed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 1;

    private Rigidbody2D rb;
    private int jumpCount;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Transform currentPlatform = null;
    private Vector3 lastPlatformPosition;

    [Header("地面檢測")] public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("夜視模式（雞關卡）")] public Light2D globalLight;
    public float nightVisionDuration = 5f;
    private float nightVisionCooldown = 0f;
    private bool canUseNightVision = false;
    private bool isNightVisionActive = false;

    [Header("蛇關卡 - Dash（地面 & 空中衝刺）")] private bool canDash = false;
    private bool isDashing = false;
    private float dashCooldown = 0f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

    [Header("龍關卡 - 飛行能力")] 
    public bool canFly = true; 
    private bool isFlying = false; 
    private int flyCount = 0; 
    private const int maxFlyUses = 3; 
    public float flySpeed = 5f;
    private float flyTimer = 0f; 
    private float flyCooldownTimer = 0f; 
    private bool isFlyCooldown = false; 
    private float baseGravity = 1f;

    [Header("UI 冷卻顯示")] public Image nightVisionIcon;
    public TMP_Text nightVisionCooldownText;
    public Image dashIcon;
    public TMP_Text dashCooldownText;
    public Image flyIcon;
    public TMP_Text flyCooldownText;

    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        jumpCount = maxJumps;

        string sceneName = SceneManager.GetActiveScene().name;
        flyIcon.gameObject.SetActive(false);
        flyCooldownText.gameObject.SetActive(false);
        dashIcon.gameObject.SetActive(false);
        dashCooldownText.gameObject.SetActive(false);
        nightVisionIcon.gameObject.SetActive(false);
        nightVisionCooldownText.gameObject.SetActive(false);

        if (sceneName == "Rooster")
        {
            globalLight.intensity = 0.005f;
            canUseNightVision = true;
            nightVisionIcon.gameObject.SetActive(true);
            nightVisionCooldownText.gameObject.SetActive(true);
        }
        else if (sceneName == "Goat")
        {
            maxJumps = 2;
        }
        else if (sceneName == "Snake")
        {
            canDash = true;
            dashIcon.gameObject.SetActive(true);
            dashCooldownText.gameObject.SetActive(true);
        }
        else if (sceneName == "Dragon")
        {
            canFly = true;
            flyIcon.gameObject.SetActive(true);  // ✅ 只在 Dragon 地圖顯示飛行 UI
            flyCooldownText.gameObject.SetActive(true);
        }
        else if (sceneName == "Boss")
        {
            if (PlayerPrefs.GetInt("GoatRune", 0) == 1)
            {
                maxJumps = 2; // ✅ **允許二段跳**
                Debug.Log("🐐 已解鎖二段跳！");
            }
            if (PlayerPrefs.GetInt("SnakeRune", 0) == 1)
            {
                canDash = true; // ✅ **允許 Dash**
                dashIcon.gameObject.SetActive(true);
                dashCooldownText.gameObject.SetActive(true);
                Debug.Log("🐍 已解鎖 Dash！");
            }
            if (PlayerPrefs.GetInt("RoosterRune", 0) == 1)
            {
                canUseNightVision = true; // ✅ **允許夜視模式**
                nightVisionIcon.gameObject.SetActive(true);
                nightVisionCooldownText.gameObject.SetActive(true);
                Debug.Log("🐓 已解鎖夜視模式！");
            }
            if (PlayerPrefs.GetInt("DragonRune", 0) == 1)
            {
                canFly = true; // ✅ **允許飛行**
                flyIcon.gameObject.SetActive(true);
                flyCooldownText.gameObject.SetActive(true);
                Debug.Log("🐉 已解鎖飛行！");
            }
            else
            {
                maxJumps = 1;
                flyIcon.gameObject.SetActive(false);
                flyCooldownText.gameObject.SetActive(false);
                dashIcon.gameObject.SetActive(false);
                dashCooldownText.gameObject.SetActive(false);
                nightVisionIcon.gameObject.SetActive(false);
                nightVisionCooldownText.gameObject.SetActive(false);
            }

        }
        else if (sceneName == "Tutorial")
        {
            canDash = true;
            canUseNightVision = true;
            maxJumps = 2;
            canFly = true;
            flyIcon.gameObject.SetActive(true);
            flyCooldownText.gameObject.SetActive(true);
            dashIcon.gameObject.SetActive(true);
            dashCooldownText.gameObject.SetActive(true);
            nightVisionIcon.gameObject.SetActive(true);
            nightVisionCooldownText.gameObject.SetActive(true);
           
        }


        if (globalLight == null)
        {
            globalLight = Object.FindAnyObjectByType<Light2D>();

        }
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        }

        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();

        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount--;
        }

        // ✅ **夜視技能冷卻 & 觸發**
        if (canUseNightVision && Input.GetKeyDown(KeyCode.N) && nightVisionCooldown <= 0)
        {
            ToggleNightVision();
        }

        if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && dashCooldown <= 0)
        {
            StartCoroutine(Dash());
        }

        // ✅ **Flight Activation**
        if ((SceneManager.GetActiveScene().name == "Dragon" || SceneManager.GetActiveScene().name == "Boss" || SceneManager.GetActiveScene().name == "Tutorial") && canFly)
        {
            if (!isFlying && flyCount < maxFlyUses && !isFlyCooldown)
            {
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    StartFlying();
                }
            }

            // ✅ **飛行中允許玩家控制升降**
            if (isFlying)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    rb.gravityScale = Mathf.Max(0, rb.gravityScale - 0.3f * Time.deltaTime); // ✅ 按住逐步減少重力
                }
                else
                {
                    rb.gravityScale = Mathf.Min(baseGravity, rb.gravityScale + 0.3f * Time.deltaTime); // ✅ 鬆開逐步恢復重力
                }

                flyTimer -= Time.deltaTime;
                if (flyTimer <= 0)
                {
                    StopFlying(true);
                }
            }
        }

        // ✅ 飛行冷卻計時
        if (isFlyCooldown)
        {
            flyCooldownTimer -= Time.deltaTime;
            if (flyCooldownTimer <= 0)
            {
                isFlyCooldown = false;
                flyCount++;

                if (flyCount >= maxFlyUses)
                {
                    canFly = false; // ✅ 3次後永久禁用飛行
                }
            }
        }

        // ✅ **Flight Cooldown System**
        if (isFlyCooldown)
        {
            flyCooldownTimer -= Time.deltaTime;
            if (flyCooldownTimer <= 0)
            {
                isFlyCooldown = false;
                flyCount++;

                if (flyCount >= maxFlyUses)
                {
                    canFly = false;
                }
            }
        }


        // ✅ **確保冷卻時間減少**
        if (nightVisionCooldown > 0)
        {
            nightVisionCooldown -= Time.deltaTime;
        }

        if (dashCooldown > 0)
        {
            dashCooldown -= Time.deltaTime;
        }

        UpdateCooldownUI();
    }

    void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            jumpCount = maxJumps;
        }

        if (isFlying)
        {
            float vertical = Input.GetKey(KeyCode.LeftControl) ? 1 : -1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * flySpeed); // ✅ **在 FixedUpdate 控制飛行**
        }

        // ✅ **如果站在移動平台上，玩家跟隨平台移動**
        if (currentPlatform != null)
        {
            Vector3 platformMovement = currentPlatform.position - lastPlatformPosition;
            transform.position += platformMovement;
            lastPlatformPosition = currentPlatform.position;
        }
    }

    // ✅ **確保站上移動平台後，jumpCount 正確重置**
    void OnCollisionEnter2D(Collision2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        if (collisionLayer == LayerMask.NameToLayer("Ground") ||
            collisionLayer == LayerMask.NameToLayer("Moving updGround") ||
            collisionLayer == LayerMask.NameToLayer("Moving LeftRGround") ||
            collisionLayer == LayerMask.NameToLayer("Falling Ground"))
        {
            jumpCount = maxJumps; // ✅ 站上平台時刷新跳躍次數

            // ✅ **只有站上 Moving 平台時才會讓角色跟隨平台移動**
            if (collisionLayer == LayerMask.NameToLayer("Moving updGround") ||
                collisionLayer == LayerMask.NameToLayer("Moving LeftRGround"))
            {
                currentPlatform = collision.transform;
                lastPlatformPosition = currentPlatform.position;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        // ✅ **當角色離開 Moving 平台時，取消跟隨**
        if (collisionLayer == LayerMask.NameToLayer("Moving updGround") ||
            collisionLayer == LayerMask.NameToLayer("Moving LeftRGround"))
        {
            currentPlatform = null;
        }
    }


    // ✅ **夜視模式開關**
    void ToggleNightVision()
    {
        if (!isNightVisionActive)
        {
            isNightVisionActive = true;
            globalLight.intensity = 1.5f;
            globalLight.color = new Color(1f, 1f, 0.8f);
            nightVisionCooldown = nightVisionDuration + 3f; // ✅ 設定冷卻時間（夜視時間 + 3 秒冷卻）
            StartCoroutine(NightVisionTimer());
        }
    }

    public float GetNightVisionCooldown()
    {
        return nightVisionCooldown; // ✅ 允許外部腳本讀取 nightVisionCooldown
    }

    // ✅ **夜視模式 5 秒後關閉**
    IEnumerator NightVisionTimer()
    {
        yield return new WaitForSeconds(nightVisionDuration);
        globalLight.intensity = 0.005f;
        globalLight.color = Color.white;
        isNightVisionActive = false;
    }

    // ✅ **Shift 觸發 Dash（地面 & 空中）**
    IEnumerator Dash()
    {
        isDashing = true;
        dashCooldown = 1.5f;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = facingRight ? 1f : -1f;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0);
        }
        else
        {
            rb.linearVelocity = new Vector2(dashDirection * (dashSpeed * 0.8f), rb.linearVelocity.y * 0.5f);
        }

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    // ✅ **龍關卡飛行**
    void StartFlying()
    {
        isFlying = true;
        flyTimer = 5f; // ✅ 開始倒數 5 秒
        rb.gravityScale = baseGravity; // ✅ 確保重力恢復到正常值
    }

    void StopFlying(bool naturalEnd = false)
    {
        isFlying = false;
        rb.gravityScale = baseGravity; // ✅ 恢復正常重力

        if (naturalEnd)
        {
            isFlyCooldown = true;
            flyCooldownTimer = 5f; // ✅ 進入冷卻
        }
    }
    void UpdateCooldownUI()
    {
        UpdateSkillUI(nightVisionIcon, nightVisionCooldownText, ref nightVisionCooldown);
        UpdateSkillUI(dashIcon, dashCooldownText, ref dashCooldown);
        if (isFlying)
        {
            UpdateSkillUI(flyIcon, flyCooldownText, ref flyTimer);  // ✅ 傳遞飛行時間
        }
        else if (isFlyCooldown)
        {
            UpdateSkillUI(flyIcon, flyCooldownText, ref flyCooldownTimer);  // ✅ 傳遞冷卻時間
        }
        else if (canFly)
        {
            flyCooldownText.text =  maxFlyUses - flyCount+"/3"; // ✅ 顯示剩餘次數
        }
    }

    void UpdateSkillUI(Image icon, TMP_Text cooldownText, ref float cooldown)
    {
        if (cooldown > 0)
        {
            cooldownText.text = Mathf.Ceil(cooldown).ToString();
            icon.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            cooldownText.text = "";
            icon.color = new Color(1f, 1f, 1f, 1f);
        }
    }


    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}