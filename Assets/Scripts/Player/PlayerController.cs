using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public enum DepartmentType { Developer, Marketing, Accounting }

    [Header("Data Driven Department")]
    public DepartmentData currentDepartmentData;

    [Header("Movement (Runtime Dynamic)")]
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    [Header("Stats & Progression")]
    public float currentBurnout = 0f;
    public float maxBurnout = 100f;

    [Header("Synergy System")]
    public System.Collections.Generic.List<string> chosenUpgradeIDs = new System.Collections.Generic.List<string>();
    [HideInInspector] public bool isOvertimeSynergyActive = false;    // AttackSpeed + Damage (Kahve patlaması)
    [HideInInspector] public bool isDeadlineSynergyActive = false;    // Range + Damage (Alan yavaşlatma)
    [HideInInspector] public bool isHomeOfficeSynergyActive = false;  // MoveSpeed + MaxHealth (Geri tepme)
    [HideInInspector] public bool isBrainstormSynergyActive = false;   // AttackSpeed + Range (Üçlü atış)

    [HideInInspector] public float attackCooldownModifier = 1f;

    // UI Listeners
    public static event Action<float, float> OnBurnoutChanged;
    public static event Action<float, float> OnXPChanged;
    public static event Action<int> OnLevelChanged;

    [Space]
    public int currentLevel = 1;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;

    private AutoAttack autoAttackScript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        autoAttackScript = GetComponent<AutoAttack>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
        ApplyPermanentUpgrades();
        ApplyDepartmentStats();
        InitUI();
    }

    public void ApplyDepartmentStats()
    {
        if (currentDepartmentData == null)
        {
            Debug.LogError("[HR] Karakterin Departman Verisi (currentDepartmentData) atanmamış! Lütfen Inspector'dan bir asset sürükleyin.");
            return;
        }
        moveSpeed = currentDepartmentData.moveSpeed;
        maxBurnout = currentDepartmentData.maxBurnout;
        attackCooldownModifier = currentDepartmentData.attackCooldownModifier;
        currentBurnout = 0f;

        if (autoAttackScript != null)
        {
            autoAttackScript.UpdateFireRate(attackCooldownModifier);
            if (currentDepartmentData.damageMultiplier > 1f)
            {
                autoAttackScript.UpdateDamage(currentDepartmentData.damageMultiplier);
            }
            if (currentDepartmentData.rangeBonus > 0f)
            {
                autoAttackScript.UpdateRange(currentDepartmentData.rangeBonus);
            }
        }

        Debug.LogWarning($"[HR] {currentDepartmentData.departmentName} Departmanı işe başladı! Hız: {moveSpeed}, Max Stres: {maxBurnout}");
    }
    void ApplyPermanentUpgrades()
    {
        if (DataManager.Instance == null) return;

        int damageLevel = DataManager.Instance.GetUpgradeLevel("Damage");
        int speedLevel = DataManager.Instance.GetUpgradeLevel("Speed");
        int healthLevel = DataManager.Instance.GetUpgradeLevel("Health");
        Debug.Log($"[UPGRADES] Kalıcı Yükseltmeler Uygulanıyor - Hasar: {damageLevel}, Hız: {speedLevel}, Sağlık: {healthLevel}");
    }
    private void InitUI()
    {
        Invoke("TriggerInitialUI", 0.05f);
    }
    private void TriggerInitialUI()
    {
        OnBurnoutChanged?.Invoke(currentBurnout, maxBurnout);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
        OnLevelChanged?.Invoke(currentLevel);
    }
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        if (moveInput.x > 0f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0f)
        {
            spriteRenderer.flipX = true;
        }

        if (anim != null)
        {
            bool isMoving = moveInput.magnitude > 0f;
            anim.SetBool("isWalking", isMoving);
        }
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
            AddXP(25f);
            Manager_UIManager uiManager = FindAnyObjectByType<Manager_UIManager>();
            if (uiManager != null) 
            {
                uiManager.currentLevelXP += 1;
            }
            Destroy(collision.gameObject);
        }
    }
    public void AddXP(float amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }
    void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = Mathf.Round(xpToNextLevel * 1.2f);
        OnLevelChanged?.Invoke(currentLevel);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }
    public void IncreaseBurnout(float amount)
    {
        currentBurnout += amount;
        if (currentBurnout < 0f) currentBurnout = 0f;
        OnBurnoutChanged?.Invoke(currentBurnout, maxBurnout);

        if (currentBurnout >= maxBurnout)
        {
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
        OnBurnoutChanged?.Invoke(currentBurnout, maxBurnout);
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
    public void ActivateOvertimeSynergy()
    {
        if (isOvertimeSynergyActive) return;
        isOvertimeSynergyActive = true;
        Debug.LogWarning("[SYNERGY ACTIVATED] 'Mesai Patlaması' Sinerjisi Tetiklendi! Artık kahveler patlayıcı hasar veriyor!");
    }
    public void ActivateDeadlineSynergy()
    {
        if (isDeadlineSynergyActive) return;
        isDeadlineSynergyActive = true;
        Debug.LogWarning("[SYNERGY] 'Deadline Paneli' Sinerjisi Aktif! Kahveler düşmanı yavaşlatıyor.");
    }
    public void ActivateHomeOfficeSynergy()
    {
        if (isHomeOfficeSynergyActive) return;
        isHomeOfficeSynergyActive = true;
        Debug.LogWarning("[SYNERGY] 'Home Office Lüksü' Sinerjisi Aktif! Kahveler kurumsal geri tepme (Knockback) uyguluyor.");
    }
    public void ActivateBrainstormSynergy()
    {
        if (isBrainstormSynergyActive) return;
        isBrainstormSynergyActive = true;
        if (autoAttackScript != null) autoAttackScript.UpdateFireRate(attackCooldownModifier * 0.8f);
        Debug.LogWarning("[SYNERGY] 'Brainstorming' Sinerjisi Aktif! Atışlar artık çoklu yayılıyor.");
    }
    void GameOver()
    {
        Debug.LogError("[SYSTEM] Karakter burnout oldu! İK çıkış mülakatı başlatılıyor...");
        Manager_UIManager uiManager = FindAnyObjectByType<Manager_UIManager>();
        if (uiManager != null)
        {
            uiManager.TriggerGameOverScreen();
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
}