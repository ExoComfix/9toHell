using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float fireRate = 1.5f;
    private float nextFireTime = 0f;

    [Header("Detection")]
    public float attackRange = 7f;
    public LayerMask enemyLayer;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Transform target = FindNearestEnemy();
            if (target != null)
            {
                Fire(target);
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    Transform FindNearestEnemy()
    {
        // Belirli bir çaptaki düşmanları bulur
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
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
        if (projectilePrefab == null) return;

        GameObject projObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projObj.GetComponent<Projectile>();

        if (projectile != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            projectile.Setup(direction);
        }
    }

    // Editörde menzili kırmızı bir çember olarak görmeni sağlar
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}