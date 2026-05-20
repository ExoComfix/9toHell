using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("Stats & Progression")]
    public float currentBurnout = 0f;
    public float maxBurnout = 100f;

    [HideInInspector] public float attackCooldownModifier = 1f;

    [Space]
    public int currentLevel = 1;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;

    private AutoAttack autoAttackScript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        autoAttackScript = GetComponent<AutoAttack>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("XP"))
        {
            GainXP(25f);
            Destroy(collision.gameObject);
        }
    }

    public void GainXP(float amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel *= 1.2f;
        moveSpeed += 0.5f;
        Debug.LogWarning($"TERFİ ALDIN! Seviye: {currentLevel}");
    }

    public void IncreaseBurnout(float amount)
    {
        currentBurnout += amount;
        if (currentBurnout >= maxBurnout)
        {
            currentBurnout = maxBurnout;
            GameOver();
        }
    }

    public void UpgradeAttackSpeed()
    {
        attackCooldownModifier *= 0.75f;
        if (autoAttackScript != null)
        {
            autoAttackScript.UpdateFireRate(attackCooldownModifier);
        }
    }

    public void UpgradeMoveSpeed()
    {
        moveSpeed += 1.5f;
        Debug.Log($"[UPGRADE] Yeni Hareket Hızı: {moveSpeed}");
    }

    public void UpgradeMaxHealth()
    {
        currentBurnout = 0f;
        Debug.Log("[UPGRADE] Stres sıfırlandı!");
    }
    
    public void UpgradeDamage()
    {
        if (autoAttackScript != null)
        {
            autoAttackScript.UpdateDamage(1.5f);
            Debug.Log("[UPGRADE] Saldırı gücü artırıldı!");
        }
    }
    public void UpgradeRange()
    {
        if (autoAttackScript != null)
        {
            autoAttackScript.UpdateRange(2f);
            Debug.Log("[UPGRADE] Saldırı menzili artırıldı!");
        }
    }

    private void GameOver()
    {
        Debug.Log("İSTİFA ETTİN! Game Over.");
        Time.timeScale = 0f;
    }
}