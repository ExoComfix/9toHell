using System.Collections.Generic;

using UnityEngine;

using System;



[RequireComponent(typeof(Rigidbody2D))]

[RequireComponent(typeof(PlayerInput))]

[RequireComponent(typeof(PlayerMovement))]

[RequireComponent(typeof(PlayerCombatCoordinator))]

[RequireComponent(typeof(PlayerXPProgression))]

[RequireComponent(typeof(AutoAttack))]

public class PlayerController : MonoBehaviour

{

    private const float XpPickupAmount = 25f;



    [Header("Data Driven Department")]

    public DepartmentData currentDepartmentData;



    [Header("Stats & Progression")]

    public float currentBurnout = 0f;

    public float maxBurnout = 100f;



    public static event Action<float, float> OnBurnoutChanged;

    public static event Action<float, float> OnXPChanged;

    public static event Action<int> OnLevelChanged;

    public static event Action OnPlayerDeath;



    private PlayerInput playerInput;

    private PlayerMovement playerMovement;

    private PlayerCombatCoordinator combatCoordinator;

    private PlayerXPProgression xpProgression;



    public PlayerCombatCoordinator Combat => combatCoordinator;

    public List<string> chosenUpgradeIDs => combatCoordinator.chosenUpgradeIDs;



    public float currentXP => xpProgression.currentXP;

    public int currentLevel => xpProgression.currentLevel;

    public float xpToNextLevel => xpProgression.xpToNextLevel;



    private void Awake()

    {

        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null) playerInput = gameObject.AddComponent<PlayerInput>();



        playerMovement = GetComponent<PlayerMovement>();

        if (playerMovement == null) playerMovement = gameObject.AddComponent<PlayerMovement>();



        combatCoordinator = GetComponent<PlayerCombatCoordinator>();

        if (combatCoordinator == null) combatCoordinator = gameObject.AddComponent<PlayerCombatCoordinator>();



        xpProgression = GetComponent<PlayerXPProgression>();

        if (xpProgression == null) xpProgression = gameObject.AddComponent<PlayerXPProgression>();



        xpProgression.BindEvents(

            (xp, toNext) => OnXPChanged?.Invoke(xp, toNext),

            level => OnLevelChanged?.Invoke(level));

    }



    private void Start()

    {

        playerMovement.ConfigureRigidbody();

        ApplyPermanentUpgrades();

        ApplyDepartmentStats();

        InitUI();

    }



    private void FixedUpdate()

    {

        playerMovement.ApplyVelocity(playerInput.MoveInput);

    }



    public void ApplyDepartmentStats()

    {

        if (currentDepartmentData == null)

        {

            Debug.LogError("[HR] Karakterin Departman Verisi (currentDepartmentData) atanmamış!");

            return;

        }



        playerMovement.SetMoveSpeed(currentDepartmentData.moveSpeed);

        maxBurnout = currentDepartmentData.maxBurnout;

        currentBurnout = 0f;



        combatCoordinator.ApplyDepartmentWeaponStats(currentDepartmentData);



        Debug.LogWarning($"[HR] {currentDepartmentData.departmentName} Departmanı işe başladı! Hız: {playerMovement.MoveSpeed}");

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

        xpProgression.NotifyInitialState();

    }



    private void OnTriggerEnter2D(Collider2D collision)

    {

        if (collision.CompareTag("XP"))

        {

            AddXP(XpPickupAmount);

            if (ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.ReturnToPool(collision.gameObject);
            }
            else
            {
                collision.gameObject.SetActive(false);
            }

        }

    }



    public void AddXP(float amount)

    {

        xpProgression.AddXP(amount);

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



    public void ModifyMoveSpeed(float amount)

    {

        playerMovement.AddMoveSpeed(amount);

    }



    public void ResetBurnout()

    {

        currentBurnout = 0f;

        OnBurnoutChanged?.Invoke(currentBurnout, maxBurnout);

    }



    #region Combat facades (preserves existing callers; logic lives in PlayerCombatCoordinator)



    public void ApplyUpgradeToWeapon(string upgradeType, float value)

    {

        combatCoordinator.ApplyUpgradeToWeapon(upgradeType, value);

    }



    public void ActivateSynergy(string synergyID)

    {

        combatCoordinator.ActivateSynergy(synergyID);

    }



    public bool IsSynergyActive(string synergyID)

    {

        return combatCoordinator.IsSynergyActive(synergyID);

    }



    #endregion



    private void GameOver()

    {

        Debug.LogError("[SYSTEM] Karakter burnout oldu! İK çıkış mülakatı başlatılıyor...");

        OnPlayerDeath?.Invoke();

    }

}


