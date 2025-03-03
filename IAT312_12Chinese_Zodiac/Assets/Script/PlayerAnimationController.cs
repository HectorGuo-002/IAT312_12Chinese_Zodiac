using UnityEngine;
using System.Collections;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip jumpSound;
    private PlayerController playerController;
    private bool facingRight = true;
    private float originalScaleX;
    private bool isJumping = false;
    private bool isAttacking = false; // ✅ 追蹤攻擊狀態
    private bool wasWalkingBeforeAttack = false; // ✅ 記錄攻擊前是否在行走

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();
        originalScaleX = Mathf.Abs(transform.localScale.x);

        if (anim == null) Debug.LogError("❌ Animator 未找到！請確保 Player 物件上有 Animator 組件！");
        if (playerController == null) Debug.LogError("❌ PlayerController 未找到！請確保 Player 物件上有 PlayerController 組件！");
    }

    void Update()
    {
        if (!isAttacking) // ✅ **攻擊時不允許行走動畫播放**
        {
            UpdateMovementAnimation();
            UpdateJumpAnimation();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            StartCoroutine(PlayAttackAnimation()); // ✅ **啟動攻擊動畫**
        }

        FlipSprite();
    }

    /// <summary>
    /// ✅ **行走動畫**
    /// </summary>
    public void UpdateMovementAnimation()
    {
        if (anim == null || isAttacking) return; // ✅ **攻擊時不執行行走動畫**
        
        bool isWalking = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        anim.SetBool("isWalking", isWalking);
    }

    /// <summary>
    /// ✅ **跳躍動畫**
    /// </summary>
    public void UpdateJumpAnimation()
    {
        if (anim == null || isAttacking) return; // ✅ **攻擊時不執行跳躍動畫**

        bool isJumping = Input.GetKey(KeyCode.UpArrow);
        anim.SetBool("isJumping", isJumping);

        if (jumpSound != null && audioSource != null && isJumping)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    /// <summary>
    /// ✅ **攻擊動畫**
    /// </summary>
    IEnumerator PlayAttackAnimation()
    {
        isAttacking = true; // ✅ **鎖住攻擊狀態**
        wasWalkingBeforeAttack = anim.GetBool("isWalking"); // ✅ 記錄攻擊前是否在行走
        anim.SetBool("isWalking", false); // ✅ **停止行走動畫**
        
        anim.SetTrigger("attackTrigger"); // ✅ **播放攻擊動畫**
        
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // ✅ **等待攻擊動畫播放完畢**
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false; // ✅ **恢復正常狀態**

        // ✅ **如果攻擊前正在行走，恢復行走動畫**
        anim.SetBool("isWalking", wasWalkingBeforeAttack);
    }

    /// <summary>
    /// ✅ **翻轉角色**
    /// </summary>
    public void FlipSprite()
    {
        if (anim == null || isAttacking) return; // ✅ **攻擊時不翻轉角色方向**

        if (Input.GetKey(KeyCode.RightArrow) && !facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    public bool IsFacingRight()
    {
        return facingRight;
    }
}
