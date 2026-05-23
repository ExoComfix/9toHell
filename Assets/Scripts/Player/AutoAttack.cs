using UnityEngine;
using System.Collections;

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

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        InitializeWeapon();
        StartCoroutine(ScanForTargetsRoutine());
    }

    public void InitializeWeapon()
    {
        if (currentWeaponData != null)
        {
            currentFireRate = currentWeaponData.baseFireRate;
            dynamicRange = currentWeaponData.attackRange;
        }
    }

    private void Update()
    {
        if (currentWeaponData == null) return;

        if (Time.time >= nextFireTime)
        {
            if (currentNearestEnemy != null)
            {
                ExecuteAttackPattern(currentNearestEnemy);
                nextFireTime = Time.time + currentFireRate;
            }
        }
    }
    private IEnumerator ScanForTargetsRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(targetScanRate);
        while (true)
        {
            if (currentWeaponData != null)
            {
                currentNearestEnemy = FindNearestEnemy();
            }
            yield return wait;
        }
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, dynamicRange, enemyLayer);
        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in hitEnemies)
        {
            float distanceToEnemy = (transform.position - enemy.transform.position).sqrMagnitude;
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }
        return nearestEnemy;
    }

    private void ExecuteAttackPattern(Transform target)
    {
        if (ObjectPooler.Instance == null || currentWeaponData == null) return;

        Vector3 baseDirection = (target.position - transform.position).normalized;
        if (playerController != null && playerController.IsSynergyActive("Brainstorm"))
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
        GameObject projObj = ObjectPooler.Instance.SpawnFromPool(currentWeaponData.poolTag, transform.position, Quaternion.identity);

        if (projObj != null)
        {
            Projectile projectile = projObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Setup(direction, playerController);
                projectile.damage = currentWeaponData.baseDamage * damageModifier;
                projectile.speed = currentWeaponData.projectileSpeed;
                projectile.lifetime = currentWeaponData.lifetime;
            }
        }
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