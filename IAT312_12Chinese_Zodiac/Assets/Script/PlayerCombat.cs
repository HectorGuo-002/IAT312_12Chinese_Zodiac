using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject inkProjectilePrefab; // 墨汁预制体
    public Transform firePoint; // 发射点
    public float inkSpeed = 10f; // 墨汁速度

    private SpriteRenderer spriteRenderer; // 确保角色翻转正确

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

       
      
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootInk();
        }
    }

    void ShootInk()
    {

        // **生成墨汁**
        GameObject ink = Instantiate(inkProjectilePrefab, firePoint.position, Quaternion.identity);

        // **确保墨汁不会继承 Player 的 Scale**
        ink.transform.SetParent(null);
        ink.transform.localScale = new Vector3(0.3f, 0.3f, 0f);

        // **获取 Rigidbody2D**
        Rigidbody2D rb = ink.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody2D 未找到！请确保 `InkProjectile` 预制体上有 `Rigidbody2D` 组件！");
            return;
        }

        // **获取角色朝向**
        float direction = spriteRenderer.flipX ? -1f : 1f;

        // **让子弹朝着角色面向方向移动**
        rb.linearVelocity = new Vector2(direction * inkSpeed, 0);
    }
}