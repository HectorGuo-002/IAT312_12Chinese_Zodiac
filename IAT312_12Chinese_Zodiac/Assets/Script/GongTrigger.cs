using UnityEngine;

public class GongTrigger : MonoBehaviour
{
    public HiddenBlock hiddenBlock; // ✅ 連接要顯示的隱藏板塊
    private Animator animator; // 🎬 Gong 动画
    
    void Start()
    {
        // **获取 Gong 的 Animator 组件**
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("❌ Animator 未找到！请确保 Gong 物体有 `Animator` 组件！");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("InkProjectile")) // 🚀 確保被墨汁擊中
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit"); // 触发 `Hit` 动画参数
            }
            if (hiddenBlock != null)
            {
                
                hiddenBlock.ActivateBlock(); // ✅ 讓隱藏板塊顯示
                Debug.Log("🎵 鑼鼓被擊中！隱藏板塊出現！");
            }
            else
            {
                Debug.LogError("❌ HiddenBlock 未連接！請在 Inspector 連接隱藏板塊。");
            }

            Destroy(collision.gameObject); // ✅ 墨汁子彈消失
        }
    }
}