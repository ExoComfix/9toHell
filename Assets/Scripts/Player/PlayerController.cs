using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Data Driven Department")]
    public DepartmentData currentDepartmentData;

    [Header("Movement (Runtime Dynamic)")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    [Header("Stats & Progression")]
    public float currentBurnout = 0f;
    public float maxBurnout = 100f;
    public int currentLevel = 1;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;

    [HideInInspector] public float attackCooldownModifier = 1f;

    [Header("Synergy & Upgrades Trackers")]
    private HashSet<string> activeSynergies = new HashSet<string>();
    public List<string> chosenUpgradeIDs = new List<string>();

    // UI & Game Event Listeners
    public static event Action<float, float> OnBurnoutChanged;
    public static event Action<float, float> OnXPChanged;
    public static event Action<int> OnLevelChanged;
    public static event Action OnPlayerDeath;

    private AutoAttack autoAttackScript;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        autoAttackScript = GetComponent<AutoAttack>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        ConfigureRigidbody();
        ApplyPermanentUpgrades();
        ApplyDepartmentStats();
        InitUI();
    }

    private void ConfigureRigidbody()
    {
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    public void ApplyDepartmentStats()
    {
        if (currentDepartmentData == null)
        {
            Debug.LogError("[HR] Karakterin Departman Verisi (currentDepartmentData) atanmamış!");
            return;
        }

        moveSpeed = currentDepartmentData.moveSpeed;
        maxBurnout = currentDepartmentData.maxBurnout;
        attackCooldownModifier = currentDepartmentData.attackCooldownModifier;
        currentBurnout = 0f;

        if (autoAttackScript != null)
        {
            autoAttackScript.UpdateFireRate(attackCooldownModifier);
            if (currentDepartmentData.damageMultiplier > 1f) autoAttackScript.UpdateDamage(currentDepartmentData.damageMultiplier);
            if (currentDepartmentData.rangeBonus > 0f) autoAttackScript.UpdateRange(currentDepartmentData.rangeBonus);
        }

        Debug.LogWarning($"[HR] {currentDepartmentData.departmentName} Departmanı işe başladı! Hız: {moveSpeed}");
    }

    private void ApplyPermanentUpgrades()
    {
        if (DataManager.Instance == null) return;

        int damageLevel = DataManager.Instance.GetUpgradeLevel("Damage");
        int speedLevel = DataManager.Instance.GetUpgradeLevel("Speed");
        int healthLevel = DataManager.Instance.GetUpgradeLevel("Health");
    }

    private void InitUI()
    {
        Invoke(nameof(TriggerInitialUI), 0.05f);
    }

    private void TriggerInitialUI()
    {
        OnBurnoutChanged?.Invoke(currentBurnout, maxBurnout);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
        OnLevelChanged?.Invoke(currentLevel);
    }

    private void Update()
    {
        HandleInput();
        HandleAnims();
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    private void HandleInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        if (moveInput.x > 0f) spriteRenderer.flipX = false;
        else if (moveInput.x < 0f) spriteRenderer.flipX = true;
    }
    private void HandleAnims()
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", moveInput.sqrMagnitude > 0f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("XP"))
        {
            AddXP(25f);
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
    private void LevelUp()
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
        currentBurnout = Mathf.Clamp(currentBurnout, 0f, maxBurnout);
        OnBurnoutChanged?.Invoke(currentBurnout, maxBurnout);

        if (currentBurnout >= maxBurnout)
        {
            GameOver();
        }
    }
    #region MODULAR UPGRADE & SYNERGY SYSTEM

    public void ModifyMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }
    public void ResetBurnout()
    {
        currentBurnout = 0f;
        OnBurnoutChanged?.Invoke(currentBurnout, maxBurnout);
    }
    public void ApplyUpgradeToWeapon(string upgradeType, float value)
    {
        if (autoAttackScript == null) return;

        switch (upgradeType)
        {
            case "Damage": autoAttackScript.UpdateDamage(value); break;
            case "Range": autoAttackScript.UpdateRange(value); break;
            case "FireRate": autoAttackScript.UpdateFireRate(value); break;
        }
    }
    public void ActivateSynergy(string synergyID)
    {
        if (activeSynergies.Contains(synergyID)) return;

        activeSynergies.Add(synergyID);
        Debug.LogWarning($"[SYNERGY ACTIVATED] {synergyID} sinerjisi başarıyla devreye alındı!");
    }

    public bool IsSynergyActive(string synergyID)
    {
        return activeSynergies.Contains(synergyID);
    }
    #endregion
    private void GameOver()
    {
        Debug.LogError("[SYSTEM] Karakter burnout oldu! İK çıkış mülakatı başlatılıyor...");
        OnPlayerDeath?.Invoke();
    }
}