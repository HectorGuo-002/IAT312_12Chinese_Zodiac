using UnityEngine;

public class RoosterAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // 获取 Animator 组件
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("❌ Animator 未找到！请确保 GameObject 上有 `Animator` 组件！");
            return;
        }

        // 播放 RoosterAnimation
        animator.Play("RoosterAnimation");
    }
}