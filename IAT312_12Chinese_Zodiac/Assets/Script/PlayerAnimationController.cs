using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private PlayerController playerController;
    private bool facingRight = true;
    private float originalScaleX;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        originalScaleX = Mathf.Abs(transform.localScale.x); // 確保原始 X 軸縮放值為正數

        if (anim == null)
        {
            Debug.LogError("❌ Animator 未找到！請確保 Player 物件上有 Animator 組件！");
        }
        if (playerController == null)
        {
            Debug.LogError("❌ PlayerController 未找到！請確保 Player 物件上有 PlayerController 組件！");
        }
    }

    void Update()
    {
        UpdateMovementAnimation();
        UpdateJumpAnimation();
        FlipSprite();
    }

    /// <summary>
    /// ✅ 只有當玩家按下 `LeftArrow` 或 `RightArrow` 才播放走路動畫
    /// </summary>
    public void UpdateMovementAnimation()
    {
        if (anim == null) return;

        bool isWalking = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        anim.SetBool("isWalking", isWalking);
    }

    /// <summary>
    /// ✅ 只有當玩家按住 `UpArrow` 時，才會播放跳躍動畫
    /// </summary>
    public void UpdateJumpAnimation()
    {
        if (anim == null) return;

        bool isJumping = Input.GetKey(KeyCode.UpArrow); // ✅ 只有當玩家按住上方向鍵時才播放跳躍動畫
        anim.SetBool("isJumping", isJumping);
    }

    /// <summary>
    /// ✅ 根據玩家的左右移動，翻轉角色
    /// </summary>
    public void FlipSprite()
    {
        if (anim == null) return;

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
}
