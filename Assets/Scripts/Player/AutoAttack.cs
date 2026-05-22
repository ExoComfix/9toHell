using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("Data Driven Weapon")]
    public WeaponData currentWeaponData;
    private float currentFireRate;
    private float dynamicRange;
    private float damageModifier = 1f;
    private float nextFireTime = 0f;

    [Header("Detection Settings")]
    public LayerMask enemyLayer;

    void Start()
    {
        if (currentWeaponData != null)
        {
            currentFireRate = currentWeaponData.baseFireRate;
            dynamicRange = currentWeaponData.attackRange;
        }
    }
    void Update()
    {
        if (currentWeaponData == null) return;

        if (Time.time >= nextFireTime)
        {
            Transform target = FindNearestEnemy();
            if (target != null)
            {
                Fire(target);
                nextFireTime = Time.time + currentFireRate;
            }
        }
    }
    public void UpdateFireRate(float modifier)
    {
        if (currentWeaponData == null) return;
        currentFireRate = currentWeaponData.baseFireRate * modifier;
        Debug.Log($"[WEAPON] Yeni Atış Bekleme Süresi: {currentFireRate} saniye.");
    }
    public void UpdateDamage(float multiplier)
    {
        damageModifier *= multiplier;
        Debug.Log($"[WEAPON] Kahve vuruş gücü (Hasar Çarpanı) artırıldı: {damageModifier}");
    }
    public void UpdateRange(float bonusRange)
    {
        dynamicRange += bonusRange;
        Debug.Log($"[WEAPON] Yeni Tarama Menzili: {dynamicRange}");
    }
    Transform FindNearestEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, dynamicRange, enemyLayer);
        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        foreach (Collider2D enemy in hitEnemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }
        return nearestEnemy;
    }
    void Fire(Transform target)
    {
        if (ObjectPooler.Instance == null || currentWeaponData == null) return;
        GameObject projObj = ObjectPooler.Instance.SpawnFromPool("PlayerCoffee", transform.position, Quaternion.identity);

        if (projObj != null)
        {
            Projectile projectile = projObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                projectile.Setup(direction);
                projectile.damage = currentWeaponData.baseDamage * damageModifier;
                projectile.speed = currentWeaponData.projectileSpeed;
                projectile.lifetime = currentWeaponData.lifetime;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, currentWeaponData != null ? dynamicRange : 7f);
    }
}