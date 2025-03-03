using UnityEngine;

public class GoatAnimationController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    public AudioClip walkSound;
    private float cooldown = 3f;
    private float cooldownTimer = 0f;
    

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (anim == null)
        {
            Debug.LogError("❌ Animator 未找到！请确保 Goat 物件上有 Animator 组件！");
        }
        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody2D 未找到！请确保 Goat 物件上有 Rigidbody2D 组件！");
        }

        audioSource.playOnAwake = false;
        
    }

    void Update()
    {
        // 读取 Rigidbody2D 的速度来判断是否播放 walk 动画
        bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.1f || Mathf.Abs(rb.linearVelocity.y) > 0.1f;
        anim.SetBool("isWalking", isWalking);
        UpdateSound(isWalking);
    }
    void UpdateSound(bool isWalking)
    {
        // 只有在 isWalking 状态下，并且冷却时间到达时，才播放羊叫声
        if (isWalking && cooldownTimer <= 0f)
        {
            audioSource.PlayOneShot(walkSound); // ✅ 播放羊叫
            cooldownTimer = cooldown; // 重置冷却时间
        }

        // 更新冷却计时器
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
}


