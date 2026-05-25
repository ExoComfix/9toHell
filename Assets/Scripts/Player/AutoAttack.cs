using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerCombatCoordinator))]
[DisallowMultipleComponent]
public class AutoAttack : MonoBehaviour
{
    [Header("Data Driven Weapon")]
    public WeaponData currentWeaponData;
    [SerializeField] private float currentFireRate;
    [SerializeField] private float dynamicRange;
    [SerializeField] private float damageModifier = 1f;
    private float nextFireTime = 0f;

    [Header("Detection Settings")]
    public LayerMask enemyLayer;
    [Tooltip("Düşman tarama sıklığı (Saniye). Performans için her frame tarama yapılmaz.")]
    [SerializeField] private float targetScanRate = 0.1f;

    private Transform currentNearestEnemy;
    private PlayerController playerController;
    private PlayerCombatCoordinator combatCoordinator;
    private EnemyTargetScanner targetScanner;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        combatCoordinator = GetComponent<PlayerCombatCoordinator>();
        targetScanner = new EnemyTargetScanner(transform, enemyLayer, () => dynamicRange);
    }

    private void Start()
    {
        InitializeWeapon();
        StartCoroutine(ScanForTargetsRoutine());
    }

    public void InitializeWeapon()
    {
        if (currentWeaponData == null) return;

        currentFireRate = currentWeaponData.baseFireRate;
        dynamicRange = currentWeaponData.attackRange;
    }

    private void Update()
    {
        if (currentWeaponData == null) return;
        if (Time.time < nextFireTime) return;
        if (currentNearestEnemy == null) return;

        ExecuteAttackPattern(currentNearestEnemy);
        nextFireTime = Time.time + currentFireRate;
    }

    private IEnumerator ScanForTargetsRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(targetScanRate);
        while (true)
        {
            if (currentWeaponData != null)
            {
                currentNearestEnemy = targetScanner.FindNearest();
            }
            yield return wait;
        }
    }

    private void ExecuteAttackPattern(Transform target)
    {
        if (currentWeaponData == null) return;

        Vector3 baseDirection = (target.position - transform.position).normalized;
        if (combatCoordinator != null && combatCoordinator.IsSynergyActive(CombatSynergyIds.Brainstorm))
        {
            FireProjectile(baseDirection);
            FireProjectile(Quaternion.Euler(0, 0, 15) * baseDirection);
            FireProjectile(Quaternion.Euler(0, 0, -15) * baseDirection);
        }
        else
        {
            FireProjectile(baseDirection);
        }
    }

    private void FireProjectile(Vector3 direction)
    {
        ProjectileLauncher.Fire(
            currentWeaponData,
            damageModifier,
            transform.position,
            direction,
            playerController,
            combatCoordinator);
    }

    #region MUTATORS (YÜKSELTME SİSTEMLERİ İÇİN)

    public void UpdateFireRate(float modifier)
    {
        if (currentWeaponData == null) return;
        currentFireRate = currentWeaponData.baseFireRate * modifier;
    }

    public void UpdateDamage(float multiplier)
    {
        damageModifier *= multiplier;
    }

    public void UpdateRange(float bonusRange)
    {
        dynamicRange += bonusRange;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, currentWeaponData != null ? dynamicRange : 7f);
    }
}
